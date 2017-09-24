using Audio;
using GUI_2;
using System;
using UnityEngine;

using System.Diagnostics;


public class BlockLogicGateMain : BlockPowered
{	
    private static bool showDebugLog = true;
	private int metaIndexPowered = 0;
	private int metaIndexTriggered = 1;
	private int metaIndexOutMode = 2;

	public static void DebugMsg(string msg)
	{
		if(showDebugLog)
		{
            String msg2 = String.Concat("SDX BlockLogicGateMain.", msg);
			UnityEngine.Debug.Log(msg2);
		}
	}
    
	private BlockActivationCommand[] MU = new BlockActivationCommand[]
	{
		new BlockActivationCommand("light", "electric_switch", false),
		new BlockActivationCommand("take", "hand", false)
	};
    
    // gets called onFocus of the switch 
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string keybindString = UIUtils.GetKeybindString(playerInput.Activate, playerInput.PermanentActions.Activate);
        if (getBitBool(_blockValue.meta, metaIndexOutMode))
		{
			return "<E> toggle out Active High";
		}
        return  "<E> toggle out Active Low";
    }
    
	// calls when player use the block
	public override bool OnBlockActivated(int _indexInBlockActivationCommands, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)
	{
        DebugMsg("OnBlockActivated");
		DebugMsg(String.Concat("OnBlockActivated  _indexInBlockActivationCommands=", _indexInBlockActivationCommands));

		StackTrace st = new StackTrace(1, true);
		StackFrame [] stFrames = st.GetFrames();
		for(int i=0; i < stFrames.Length; i++ )
		{
		   DebugMsg(String.Concat("OnBlockActivated CallingMethod:",stFrames[i].GetMethod() ));
		}
		
		if (_indexInBlockActivationCommands != 0)
		{
			if (_indexInBlockActivationCommands != 1)
			{
				return false;
			}
			base.TakeItemWithTimer(_cIdx, _blockPos, _blockValue, _player);
			return true;
		}
		else
		{
			if (!(_world.GetTileEntity(_cIdx, _blockPos) is TileEntityPoweredTrigger))
			{
				return false;
			}
			ToggleOutputMode(_world, _cIdx, _blockPos, _blockValue);
			//this.XR(_world, _cIdx, _blockPos, _blockValue);
			return true;
		}
	}
	
	private void ToggleOutputMode(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
        DebugMsg("ToggleOutputMode");
		_blockValue.meta = flipBit(_blockValue.meta, metaIndexOutMode); //^= (1 << 3); //8;
		
		bool newMode = getBitBool(_blockValue.meta, metaIndexOutMode);
		DebugMsg(String.Concat("ToggleOutputMode newMode=", newMode ? "1" : "0"));
		
		bool BlockIsTriggered = getBitBool(_blockValue.meta, metaIndexTriggered); 
		bool newIsTriggered = false;
			
		TileEntityPoweredTrigger tileEntityPoweredTrigger = _world.GetTileEntity(_cIdx, _blockPos) as 	TileEntityPoweredTrigger;
		if (tileEntityPoweredTrigger != null)
		{
			if(BlockIsTriggered)
			{	
				newIsTriggered = !newMode;				
			}
			else
			{
				newIsTriggered = newMode;
			}			
			tileEntityPoweredTrigger.IsTriggered = newIsTriggered;
			_blockValue.meta = setBoolBit(_blockValue.meta, 1, newIsTriggered); 
		}
		_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);			
		this.XR(_world, _cIdx, _blockPos, _blockValue);
	}

	private byte flipBit(byte b, int i)
	{
		b ^= (byte)(1 << i);
		return b;
	}
	
	private bool getBitBool(byte b, int i)
	{
		int mask = 1 << i;
		return (b & mask) != 0;
	}
	
		private byte setBoolBit(byte b, int i, bool val)
        {
            byte mask = (byte)(1 << i);
            if (val)
            {
                b |= mask;
            }
            else if(!val)
            {
                b &= (byte)~mask; 
            }
            return b;
        }

	// 
	private bool XR(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
        DebugMsg("XR");
		ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
		if (chunkCluster == null)
		{
			return false;
		}
		if (chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z)) == null)
		{
			return false;
		}

		bool[] inputStates = GetInputStates(_world, _cIdx, _blockPos, _blockValue);
		
		bool BlockIsPowered = getBitBool(_blockValue.meta, metaIndexPowered); //(_blockValue.meta & 1) != 0;
		bool BlockIsTriggered = getBitBool(_blockValue.meta, metaIndexTriggered); //(_blockValue.meta & 2) != 0;		
		bool outputMode =  getBitBool(_blockValue.meta, metaIndexOutMode); //(_blockValue.meta & 4) != 0; 
		
		DebugMsg(String.Concat("XR _blockvalue.meta=", Convert.ToString(_blockValue.meta, 2).PadLeft(8, '0')));
		DebugMsg(String.Concat("XR BlockMetaPowered=", BlockIsPowered  ? "1" : "0"));
		DebugMsg(String.Concat("XR BlockMetaTriggered=", BlockIsTriggered  ? "1" : "0"));
		DebugMsg(String.Concat("XR outputMode=", outputMode  ? "1" : "0"));

		DebugMsg(String.Concat("XR _blockValue.rotation=", _blockValue.rotation));
		
        bool flagOnBlockActivated = false; // this disables toggle
		if (flagOnBlockActivated)
		{
			BlockIsTriggered = !BlockIsTriggered;
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | ((!BlockIsTriggered) ? 0 : 2));
			_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | ((!BlockIsPowered) ? 0 : 1));
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		}		

		TileEntityPoweredTrigger tileEntityPoweredTrigger = _world.GetTileEntity(_cIdx, _blockPos) as 	TileEntityPoweredTrigger;
		if (tileEntityPoweredTrigger != null)
		{	
			DebugMsg(String.Concat("XR -> tEPT.IsTriggered1=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));
			// if (Steam.Network.IsServer)
			// {
				
				// tileEntityPoweredTrigger.IsTriggered = BlockIsTriggered
				
			// }			
			DebugMsg(String.Concat("XR -> tEPT.IsTriggered2=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));
		}
		
		/* 		
			TileEntityPowered tileEntityPowered = _world.GetTileEntity(_cIdx, _blockPos) as TileEntityPowered;
			if(tileEntityPowered != null)
			{				
				DebugMsg(String.Concat("XR -> tEP.ChildCount=", tileEntityPowered.ChildCount));
				DebugMsg(String.Concat("XR -> tEP.isPowered=", tileEntityPowered.IsPowered ? "1" : "0"));
			} 
		*/
		
		BlockEntityData _ebcd = ((World)_world).ChunkClusters[_cIdx].GetBlockEntity(_blockPos);
		if (_ebcd != null && _ebcd.transform != null && _ebcd.transform.gameObject != null)
		{	
			UpdateIndicators(_ebcd, inputStates, BlockIsPowered, BlockIsTriggered, outputMode);
		}		
		return true;
	}
	
	private void UpdateIndicators(BlockEntityData _ebcd, bool[] inputStates, bool BlockIsPowered, bool BlockIsTriggered, bool outputMode)
	{
        DebugMsg("UpdateIndicators");
		GameObject IndicatorsObj = _ebcd.transform.Find("Indicators").gameObject;			

		if(IndicatorsObj == null)
		{
			DebugMsg("IndicatorsObj is null");
		}
		Transform[] Indicators = IndicatorsObj.GetComponentsInChildren<Transform>();
		if (Indicators != null)
		{
			for (int i = 0; i < Indicators.Length; i++)
			{
				GameObject Indicator = Indicators[i].gameObject;
				Color tempColor = Color.black;
				if (Indicator.name == "IndMainPower")
				{
					if (BlockIsPowered)
					{
						tempColor = Color.green;
					}
					else
					{
						tempColor = Color.black;								
					}	
				}
				if(Indicator.name == "IndInput0")
				{
					if (inputStates[0])
					{
						if(inputStates[1] && inputStates[2])
						{
							tempColor = Color.green;
						}
						else
						{
							tempColor = Color.yellow;	
						}
					}
					else
					{
						tempColor = Color.black;								
					}	
				}
				if(Indicator.name == "IndInput1")
				{
					if (inputStates[1])
					{
						if(inputStates[0] && inputStates[2])
						{
							tempColor = Color.green;
						}
						else
						{
							tempColor = Color.yellow;	
						}
					}
					else
					{
						tempColor = Color.black;								
					}						
				}
				if(Indicator.name == "IndInput2")
				{
					if (inputStates[2])
					{
						if(inputStates[0] && inputStates[1])
						{
							tempColor = Color.green;
						}
						else
						{
							tempColor = Color.yellow;	
						}
					}
					else
					{
						tempColor = Color.black;								
					}					
				}				
				if (Indicator.name == "IndOutputMode")
				{
					if (outputMode)
					{
						tempColor = Color.blue;
					}
					else
					{
						tempColor = Color.black;								
					}						
				}
				if (Indicator.name == "IndOutput")
				{
					if (BlockIsPowered && BlockIsTriggered)
					{
						tempColor = Color.green;
					}
					else
					{
						tempColor = Color.black;								
					}						
				}
				SetIndicatorColor(Indicator, tempColor);
			}
		}
	}
	
	
	static Vector3i[] GetInputLocations(Vector3i _blockPos, BlockValue _blockValue)
	{
		Vector3i inputPosA = Vector3i.zero;
		Vector3i inputPosB = Vector3i.zero;
		Vector3i inputPosC = Vector3i.zero;
		
		switch(_blockValue.rotation)
		{
			case 0:
				inputPosB = _blockPos;
				inputPosB.x = _blockPos.x-1;				
				inputPosA = inputPosB;
				inputPosA.z = inputPosB.z+1;
				inputPosC = inputPosB;		
				inputPosC.z = inputPosB.z-1;
				break;
			case 1:
				inputPosB = _blockPos;
				inputPosB.z = _blockPos.z+1;		
				inputPosA = inputPosB;
				inputPosA.x = inputPosB.x+1;
				inputPosC = inputPosB;		
				inputPosC.x = inputPosB.x-1;
				break;
			case 2:
				inputPosB = _blockPos;
				inputPosB.x = _blockPos.x+1;				
				inputPosA = inputPosB;
				inputPosA.z = inputPosB.z-1;
				inputPosC = inputPosB;		
				inputPosC.z = inputPosB.z+1;
				break;	
			case 3:
				inputPosB = _blockPos;
				inputPosB.z = _blockPos.z-1;		
				inputPosA = inputPosB;
				inputPosA.x = inputPosB.x-1;     
				inputPosC = inputPosB;		
				inputPosC.x = inputPosB.x+1;
				break;
		}
		
		Vector3i[] array = new Vector3i[3];
		array[0] = inputPosA;
		array[1] = inputPosB;
		array[2] = inputPosC;
		return array;		
	}
 
	public bool[] GetInputStates(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		bool[] inputStates = {false, false, false};
		Vector3i[] locations = GetInputLocations(_blockPos, _blockValue);
		for(int i=0; i < inputStates.Length; i++)
		{
			BlockValue inputBlockValue = _world.GetBlock(locations[i]);
			Type inputBlockType = Block.list[inputBlockValue.type].GetType();
			if(inputBlockType == typeof(BlockPowered))
			{
				TileEntityPowered tileEntityPowered = (TileEntityPowered)_world.GetTileEntity(_cIdx, locations[i]);
				if (tileEntityPowered != null)
				{
					if(tileEntityPowered.IsPowered)
					{
						inputStates[i] = true;
					}
				}
			}
		}
		return inputStates;
	}
	

	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
        DebugMsg("OnBlockEntityTransformAfterActivated");
		this.XR(_world, _cIdx, _blockPos, _blockValue);
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
	}

	public override bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
        DebugMsg("ActivateBlock");
		bool isOn2 = false;
		bool isOn3 = false;
		bool[] inputStates = GetInputStates(_world, _cIdx, _blockPos, _blockValue);
		bool outputMode =  getBitBool(_blockValue.meta, metaIndexOutMode);
			
		StackTrace st = new StackTrace(1, true);
		StackFrame [] stFrames = st.GetFrames();
		for(int i=0; i < stFrames.Length; i++ )
		{
		   DebugMsg(String.Concat("OnBlockActivated CallingMethod:",stFrames[i].GetMethod() ));
		}
		
		DebugMsg(String.Concat("ActivateBlock isPowered=", isPowered ? "1" : "0"));
		DebugMsg(String.Concat("ActivateBlock isOn=", isOn ? "1" : "0"));
		DebugMsg(String.Concat("ActivateBlock inputStates=", inputStates[0] ? "1" : "0"));
		DebugMsg(String.Concat("ActivateBlock outputMode=", outputMode ? "1" : "0"));
		

		bool inputOn = true;
		for(int i = 0; i < inputStates.Length; i++)
        if (!inputStates[i]) {
            inputOn = false;
            break;
        }
		
		if(inputOn && isPowered){
			isOn2 = true;
		}
		else
		{
			isOn2 = false;
		}		
		DebugMsg(String.Concat("ActivateBlock isOn2=", isOn2 ? "1" : "0"));		
					
		if(outputMode)
		{
			isOn3 = !isOn2;
		}
		else
		{
			isOn3 = isOn2;
		}

		TileEntityPoweredTrigger tileEntityPoweredTrigger = _world.GetTileEntity(_cIdx, _blockPos) as 	TileEntityPoweredTrigger;
		if (tileEntityPoweredTrigger != null)
		{
			if(isOn != isOn3)
			{
			tileEntityPoweredTrigger.IsTriggered = isOn3; //tileEntityPoweredTrigger.
			
			//tileEntityPoweredTrigger.Activate(isPowered, isOn2);
			}
			DebugMsg(String.Concat("ActivateBlock tileEntityPoweredTrigger.IsTriggered=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));

		}
		_blockValue.meta = setBoolBit(_blockValue.meta, 0, isPowered); //(byte)(((int)_blockValue.meta & -2) | ((!isPowered) ? 0 : 1));
		_blockValue.meta = setBoolBit(_blockValue.meta, 1, isOn3); //(byte)(((int)_blockValue.meta & -3) | ((!isOn3) ? 0 : 2));

		_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);		
		this.XR(_world, _cIdx, _blockPos, _blockValue);
		
		return true;
	}

	public override void OnBlockValueChanged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
        DebugMsg("OnBlockValueChanged");
		base.OnBlockValueChanged(_world, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		//this.XR(_world, _clrIdx, _blockPos, _newBlockValue);
		BlockEntityData _ebcd = ((World)_world).ChunkClusters[_clrIdx].GetBlockEntity(_blockPos);
		//this.HR(_ebcd, BlockSwitch.IsSwitchOn(_newBlockValue.meta), _newBlockValue);
	}

	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer());
		this.MU[0].enabled = true;
		this.MU[1].enabled = (flag && this.TakeDelay > 0f);
		return this.MU;
	}


	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		DebugMsg("CreateTileEntity");
		return new TileEntityPoweredTrigger(chunk)
		{
			TriggerType = PowerTrigger.TriggerTypes.Switch
			//TriggerType = PowerTrigger.TriggerTypes.TripWire
		};
    } 

	public static bool IsSwitchOn(byte _metadata)
	{
		DebugMsg("IsSwitchOn()");
		// Get call stack
		StackTrace stackTrace = new StackTrace();
		// Get calling method name
		DebugMsg(String.Concat("IsSwitchOn() Caller ", stackTrace.GetFrame(1).GetMethod()));
		DebugMsg(String.Concat("IsSwitchOn() Caller Caller ", stackTrace.GetFrame(2).GetMethod()));
		return (_metadata & 2) != 0;
	}

	private void HR(BlockEntityData _ebcd, bool value, BlockValue blockValue)
	{
        DebugMsg("HR");
		/* 
			Animator[] componentsInChildren;
			if (_ebcdData != null && _ebcdData.bHasTransform && (componentsInChildren = _ebcdData.transform.GetComponentsInChildren<Animator>()) != null)
			{
				Animator[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					Animator animator = array[i];
					animator.SetBool("SwitchActivated", value);
					animator.SetTrigger("SwitchTrigger");
				}
			}
		*/
	}

	public override void ForceAnimationState(BlockValue _blockValue, BlockEntityData _ebcd)
	{
        DebugMsg("ForceAnimationState");
		/*
			Animator[] componentsInChildren;
			if (_ebcd != null && _ebcd.bHasTransform && (componentsInChildren = _ebcd.transform.GetComponentsInChildren<Animator>(false)) != null)
			{
				bool flag = BlockSwitch.IsSwitchOn(_blockValue.meta);
				Animator[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					Animator animator = array[i];
					animator.SetBool("SwitchActivated", flag);
					if (flag)
					{
						animator.CrossFade("SwitchOnStatic", 0f);
					}
					else
					{
						animator.CrossFade("SwitchOffStatic", 0f);
					}
				}
			}
		*/
	}
	
/*
	// BlockPressurePlate
    public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
    {
        base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue);
        if (_blockValue.ischild)
        {
            return;
        }
        if (!(_world.GetTileEntity(_chunk.ClrIdx, _blockPos) is TileEntityPoweredTrigger))
        {
            TileEntityPowered tileEntityPowered = this.CreateTileEntity(_chunk);
            tileEntityPowered.localChunkPos = World.toBlock(_blockPos);
            tileEntityPowered.InitializePowerData();
            _chunk.AddTileEntity(tileEntityPowered);
        }
    }
*/
	
	private void SetIndicatorColor(GameObject _obj, Color color)
	{
		Renderer rend =_obj.GetComponentInChildren<Renderer>();
		rend.material.SetColor("_Emission", color);
	}
	
}
