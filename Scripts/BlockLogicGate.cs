using Audio;
using GUI_2;
using System;
using System.Linq;
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
		//DebugMsg(String.Concat("OnBlockActivated  _indexInBlockActivationCommands=", _indexInBlockActivationCommands));

		/* 
			StackTrace st = new StackTrace(1, true);
			StackFrame [] stFrames = st.GetFrames();
			for(int i=0; i < stFrames.Length; i++ )
			{
			   DebugMsg(String.Concat("OnBlockActivated CallingMethod:",stFrames[i].GetMethod() ));
			}
		*/	
 
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
			this.XR(_world, _cIdx, _blockPos, _blockValue);
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
        //DebugMsg("XR");
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

		TileEntityPoweredTrigger tileEntityPoweredTrigger = _world.GetTileEntity(_cIdx, _blockPos) as 	TileEntityPoweredTrigger;
		if (tileEntityPoweredTrigger != null)
		{	
			DebugMsg(String.Concat("XR -> tEPT.IsTriggered1=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));
			// if (Steam.Network.IsServer)
			// {
				
				// tileEntityPoweredTrigger.IsTriggered = BlockIsTriggered
				
			// }			
			//DebugMsg(String.Concat("XR -> tEPT.IsTriggered2=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));
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
        //DebugMsg("UpdateIndicators");
		GameObject IndicatorsObj = _ebcd.transform.Find("Indicators").gameObject;			

		if(IndicatorsObj == null)
		{
			DebugMsg("IndicatorsObj is null");
		}
		Transform[] Indicators = IndicatorsObj.GetComponentsInChildren<Transform>();
		if (Indicators != null)
		{
			GameObject Indicator;
			Color tempColor = Color.black;
			for (int i = 0; i < Indicators.Length; i++)
			{
				Indicator = Indicators[i].gameObject;
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
	
	
	static Vector3i[][] GetInputLocations(Vector3i _blockPos, BlockValue _blockValue)
	{
		Vector3i inputPosA0 = Vector3i.zero;
		Vector3i inputPosA1 = Vector3i.zero;
		Vector3i inputPosA2 = Vector3i.zero;
		Vector3i inputPosB0 = Vector3i.zero;
		Vector3i inputPosB1 = Vector3i.zero;
		Vector3i inputPosB2 = Vector3i.zero;
		Vector3i inputPosC0 = Vector3i.zero;
		Vector3i inputPosC1 = Vector3i.zero;
		Vector3i inputPosC2 = Vector3i.zero;
		
		inputPosB0 = _blockPos;
		inputPosB1 = _blockPos;
		inputPosB2 = _blockPos;
		
		switch(_blockValue.rotation)
		{
		case 0:
			inputPosB0.x = _blockPos.x-1;
			inputPosB1.x = _blockPos.x-2;
			inputPosB2.x = _blockPos.x-3;
			inputPosA0 = inputPosB0;
			inputPosA1 = inputPosB1;
			inputPosA2 = inputPosB2;
			inputPosA0.z = inputPosB0.z+1;
			inputPosA1.z = inputPosB1.z+1;
			inputPosA2.z = inputPosB2.z+1;
			inputPosC0 = inputPosB0;
			inputPosC1 = inputPosB1;
			inputPosC2 = inputPosB2;
			inputPosC0.z = inputPosB0.z-1;
			inputPosC1.z = inputPosB1.z-1;
			inputPosC2.z = inputPosB2.z-1;
			break;
		case 1:
			inputPosB0.z = _blockPos.z+1;
			inputPosB1.z = _blockPos.z+2;
			inputPosB2.z = _blockPos.z+3;
			inputPosA0 = inputPosB0;
			inputPosA1 = inputPosB1;
			inputPosA2 = inputPosB2;
			inputPosA0.x = inputPosB0.x+1;
			inputPosA1.x = inputPosB1.x+1;
			inputPosA2.x = inputPosB2.x+1;
			inputPosC0 = inputPosB0;
			inputPosC1 = inputPosB1;
			inputPosC2 = inputPosB2;
			inputPosC0.x = inputPosB0.x-1;
			inputPosC1.x = inputPosB1.x-1;
			inputPosC2.x = inputPosB2.x-1;
			break;
		case 2:
			inputPosB0.x = _blockPos.x+1;
			inputPosB1.x = _blockPos.x+2;
			inputPosB2.x = _blockPos.x+3;
			inputPosA0 = inputPosB0;
			inputPosA1 = inputPosB1;
			inputPosA2 = inputPosB2;
			inputPosA0.z = inputPosB0.z-1;
			inputPosA1.z = inputPosB1.z-1;
			inputPosA2.z = inputPosB2.z-1;
			inputPosC0 = inputPosB0;
			inputPosC1 = inputPosB1;
			inputPosC2 = inputPosB2;
			inputPosC0.z = inputPosB0.z+1;
			inputPosC1.z = inputPosB1.z+1;
			inputPosC2.z = inputPosB2.z+1;
			break;
		case 3:
			inputPosB0.z = _blockPos.z-1;
			inputPosB1.z = _blockPos.z-2;
			inputPosB2.z = _blockPos.z-3;
			inputPosA0 = inputPosB0;
			inputPosA1 = inputPosB1;
			inputPosA2 = inputPosB2;
			inputPosA0.x = inputPosB0.x-1;
			inputPosA1.x = inputPosB1.x-1;
			inputPosA2.x = inputPosB2.x-1;
			inputPosC0 = inputPosB0;
			inputPosC1 = inputPosB1;
			inputPosC2 = inputPosB2;
			inputPosC0.x = inputPosB0.x+1;
			inputPosC1.x = inputPosB1.x+1;
			inputPosC2.x = inputPosB2.x+1;
			break;
		}

		
		Vector3i[][] locations = new Vector3i[3][];
		// locations[0] = new Vector3i[] {inputPosA0 , inputPosB0, inputPosC0};
		// locations[1] = new Vector3i[] {inputPosA1 , inputPosB1, inputPosC1};
		// locations[2] = new Vector3i[] {inputPosA2 , inputPosB2, inputPosC2};
		locations[0] = new Vector3i[] {inputPosA0 , inputPosA1, inputPosA2};
		locations[1] = new Vector3i[] {inputPosB0 , inputPosB1, inputPosB2};
		locations[2] = new Vector3i[] {inputPosC0 , inputPosC1, inputPosC2};
		return locations;		
	}
 
	public bool[] GetInputStates(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue)
	{		
		//DebugMsg("GetInputStates");
		Vector3i[][] locations = GetInputLocations(_blockPos, _blockValue);
		
		bool[] inputStates = {false, false, false};
		for(int i=0; i < locations.Length; i++)
		{	
			// DebugMsg(String.Concat("GetInputStates newInputRow=", i));
			int nonGateBlocks = 0;
			for(int x=0; x < 3; x++)
			{				
				BlockValue inputBlockValue = _world.GetBlock(locations[i][x]);
				Type inputBlockType = Block.list[inputBlockValue.type].GetType();
				// DebugMsg(String.Concat("GetInputStates type=", inputBlockType.ToString()));
				if(inputBlockType == typeof(BlockLogicGateInput))
				{
					TileEntityPoweredBlock _te = (TileEntityPoweredBlock)_world.GetTileEntity(_cIdx, locations[i][x]);
					if (_te != null)
					{
						if(_te.IsPowered)
						{
							if(_te.IsToggled) 
							{
								inputStates[i] =  true;	
							}
						}
						else
						{
							if(!_te.IsToggled) 
							{
								inputStates[i] =  true;	
							}
						}
					}
				}
				else
				{
					nonGateBlocks++;
					//DebugMsg(String.Concat("GetInputStates nonGateBlocks=", nonGateBlocks));
				}
			}
			if(nonGateBlocks == locations.Length)
			{
				inputStates[i] =  true;	
			}
			// DebugMsg(String.Concat("GetInputStates inputRow=",  inputStates[i] ? "1" : "0"));
		}
		return inputStates;
	}

	

	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
        DebugMsg("OnBlockEntityTransformAfterActivated");
		this.XR(_world, _cIdx, _blockPos, _blockValue);
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
	}

	public override bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool outputStateOld, bool isPowered)
	{
       // DebugMsg("ActivateBlock");
		bool outputStateRaw = false;
		bool outputStateNew = false;
		bool[] inputStates = GetInputStates(_world, _cIdx, _blockPos, _blockValue);
		bool outputMode =  getBitBool(_blockValue.meta, metaIndexOutMode);
			
		// StackTrace st = new StackTrace(1, true);
		// StackFrame [] stFrames = st.GetFrames();
		// for(int i=0; i < stFrames.Length; i++ )
		// {
		   // DebugMsg(String.Concat("OnBlockActivated CallingMethod:",stFrames[i].GetMethod() ));
		// }
		
		DebugMsg(String.Concat("ActivateBlock isPowered=", isPowered ? "1" : "0"));
		DebugMsg(String.Concat("ActivateBlock outputStateOld=", outputStateOld ? "1" : "0"));
		DebugMsg(String.Concat("ActivateBlock inputStates=", string.Join("", inputStates.Select(b => b ? "1" : "0").ToArray())));
		DebugMsg(String.Concat("ActivateBlock outputMode=", outputMode ? "1" : "0"));
			
		bool inputOn = true;
		for(int i = 0; i < inputStates.Length; i++)
        if (!inputStates[i]) {
            inputOn = false;
            break;
        }
		
		//if(inputOn && isPowered)
		if(inputOn)
		{
			outputStateRaw = true;
		}
		else
		{
			outputStateRaw = false;
		}		
		DebugMsg(String.Concat("ActivateBlock outputStateRaw=", outputStateRaw ? "1" : "0"));		
					
		if(outputMode)
		{
			outputStateNew = !outputStateRaw;
		}
		else
		{
			outputStateNew = outputStateRaw;
		}
		DebugMsg(String.Concat("ActivateBlock outputStateNew=", outputStateNew ? "1" : "0"));		
		
		//if(outputStateOld != outputStateNew)
		//{
				
			TileEntityPoweredTrigger tileEntityPoweredTrigger = _world.GetTileEntity(_cIdx, _blockPos) as 	TileEntityPoweredTrigger;
			if (tileEntityPoweredTrigger != null)
			{

				tileEntityPoweredTrigger.IsTriggered = outputStateNew; //tileEntityPoweredTrigger.
				DebugMsg(String.Concat("ActivateBlock tileEntityPoweredTrigger.IsTriggered=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));
			}
			_blockValue.meta = setBoolBit(_blockValue.meta, metaIndexTriggered, outputStateNew); 

		//}
		
		
		_blockValue.meta = setBoolBit(_blockValue.meta, metaIndexPowered, (isPowered)); 
		_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);		
		this.XR(_world, _cIdx, _blockPos, _blockValue);
		return true;
	}

	public override void OnBlockValueChanged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
        //DebugMsg("OnBlockValueChanged");
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
