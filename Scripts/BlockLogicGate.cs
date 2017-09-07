using Audio;
using GUI_2;
using System;
using UnityEngine;

using System.Diagnostics;


public class BlockLogicGateMain : BlockPowered
{	
    private static bool showDebugLog = true;

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
        //string msg = String.Concat("GetActivationText - _blockValue.meta &2 :", (_blockValue.meta & 2));
        //DebugMsg(msg);
        // (_blockValue.meta &2):
        // 0 : switch is Off , 1 : switch is On
    
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string keybindString = UIUtils.GetKeybindString(playerInput.Activate, playerInput.PermanentActions.Activate);
        if ((_blockValue.meta & 2) != 0)
		{

			//return string.Format(Localization.Get("useSwitchLightOff", string.Empty), keybindString);
			return string.Format("cSwitchOff", keybindString);
		}
		//return string.Format(Localization.Get("useSwitchLightOn", string.Empty), keybindString);
        return string.Format("cSwitchOn", keybindString);
    }
    
	// calls when player use the block
	public override bool OnBlockActivated(int _indexInBlockActivationCommands, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)
	{
        DebugMsg("OnBlockActivated");
		// Get call stack
		StackTrace stackTrace = new StackTrace();

		// Get calling method name
		DebugMsg(stackTrace.GetFrame(1).GetMethod().Name);
		DebugMsg(String.Concat("blub ", stackTrace.GetFrame(2).GetMethod()));
		
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
			this.XR(_world, _cIdx, _blockPos, _blockValue, true);
			return true;
		}
	}

	// 
	private bool XR(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool flagOnBlockActivated)
	{
        DebugMsg("XR");
        if(flagOnBlockActivated == null)
        {
            flagOnBlockActivated = false;
        }
		ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
		if (chunkCluster == null)
		{
			return false;
		}
		if (chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z)) == null)
		{
			return false;
		}
		

        DebugMsg("XR2");		
				
		bool hasSecondInput = HasActivePower(_world, _cIdx, _blockPos);
		
<<<<<<< HEAD
		bool BlockIsPowered = (blockValue.meta & 1) != 0; // seems to be BlockIsPowered
		bool BlockIsTriggered = (blockValue.meta & 2) != 0; // seems to be SwitchIsOn 
		
		string BlockIsPoweredmsg = BlockIsPowered ? "true" : "false";
		string BlockIsTriggeredmsg = BlockIsTriggered ? "true" : "false"; 
		string msg = "XR - BlockValue.meta &2=";
		msg = String.Concat(msg, BlockIsPoweredmsg);
=======
		bool flag2 = (_blockValue.meta & 1) != 0; // seems to be BlockIsPowered
		bool flag3 = (_blockValue.meta & 2) != 0; // seems to be SwitchIsOn 
		
		string flag2msg = flag2 ? "true" : "false";
		string flag3msg = flag3 ? "true" : "false"; 
		string msg = "XR - _blockValue.meta &2=";
		msg = String.Concat(msg, flag2msg);
>>>>>>> 40d3c5c5f734ea212ca9546317df2e32453eacc7
		msg = String.Concat(msg, " &3=");
		msg = String.Concat(msg, BlockIsTriggeredmsg);
		DebugMsg(msg);
		
		
        DebugMsg("XR3");
		
        flagOnBlockActivated = false; // this disables runtimetoggle
		if (flagOnBlockActivated)
		{
<<<<<<< HEAD
			BlockIsTriggered = !BlockIsTriggered;
			blockValue.meta = (byte)(((int)blockValue.meta & -3) | ((!BlockIsTriggered) ? 0 : 2));
			blockValue.meta = (byte)(((int)blockValue.meta & -2) | ((!BlockIsPowered) ? 0 : 1));
			worldBase.SetBlockRPC(num, vector3i, blockValue);
			if (BlockIsTriggered)
=======
			flag3 = !flag3;
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | ((!flag3) ? 0 : 2));
			_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | ((!flag2) ? 0 : 1));
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
			if (flag3)
>>>>>>> 40d3c5c5f734ea212ca9546317df2e32453eacc7
			{
				Manager.BroadcastPlay(_blockPos.ToVector3(), "switch_up");
			}
			else
			{
				Manager.BroadcastPlay(_blockPos.ToVector3(), "switch_down");
			}
		}		

		
        DebugMsg("XR4");

		TileEntityPoweredTrigger tileEntityPoweredTrigger = _world.GetTileEntity(_cIdx, _blockPos) as 	TileEntityPoweredTrigger;
		if (tileEntityPoweredTrigger != null)
		{	
			DebugMsg(String.Concat("XR -> tEPT.IsTriggered1=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));	
			// if (Steam.Network.IsServer)
			// {
				tileEntityPoweredTrigger.IsTriggered = BlockIsTriggered;
			// }			
			DebugMsg(String.Concat("XR -> tEPT.IsTriggered2=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));	
			//tileEntityPoweredTrigger.ResetTrigger();
<<<<<<< HEAD
=======
			
			DebugMsg(String.Concat("XR -> tEPT.IsTriggered3=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));	
            

>>>>>>> 40d3c5c5f734ea212ca9546317df2e32453eacc7
		}
		
		
        DebugMsg("XR5");
		
		// 		
		TileEntityPowered tileEntityPowered = _world.GetTileEntity(_cIdx, _blockPos) as TileEntityPowered;
		if(tileEntityPowered != null)
		{				
			PowerItem powerItem = tileEntityPowered.GetPowerItem() as PowerItem;
			if(powerItem != null)
			{
				// crashes the game 
				//powerItem.HandlePowerUpdate(BlockIsTriggered);
			}
			
			PowerTrigger powerTrigger = tileEntityPowered.GetPowerItem() as PowerTrigger;
			if(powerTrigger != null)
			{		
				// crashes the game 
<<<<<<< HEAD
				//powerTrigger.HandlePowerUpdate(BlockIsTriggered);
			}
			
        /*
            PowerTrigger powerTrigger = tileEntityPowered.GetPowerItem() as PowerTrigger;
			if(powerTrigger != null)
			{   
                
				powerTrigger.isTriggered = BlockIsTriggered;	
			}
        */            
=======
				//powerTrigger.HandlePowerUpdate(flag3);
			}         
>>>>>>> 40d3c5c5f734ea212ca9546317df2e32453eacc7
            
			DebugMsg(String.Concat("XR -> tEP.ChildCount=", tileEntityPowered.ChildCount));
			DebugMsg(String.Concat("XR -> tEP.isPowered=", tileEntityPowered.IsPowered ? "1" : "0"));
            
            // 
            //tileEntityPowered.;
			
			// not sure
			//tileEntityPowered.CheckForNewWires();
			
			// don't do anything noticeable
			//tileEntityPowered.MarkChanged();
            
            // don't do anything noticeable
            //tileEntityPowered.CheckForNewWires();
		}
        
    /*     
        TileEntity tileEntity = _world.GetTileEntity(_cIdx, _blockPos) as 	TileEntity;
		if (tileEntity != null)
		{
            // don't do anything noticeable
			//tileEntity.SetModified();
        }
      */   
        
		
        DebugMsg("XR6");
		
		BlockEntityData _ebcd = ((World)_world).ChunkClusters[_cIdx].GetBlockEntity(_blockPos);
		if (_ebcd != null && _ebcd.transform != null && _ebcd.transform.gameObject != null)
		{	
            // only works once per world
            //GameObject IndicatorsObj = GameObject.Find("Indicators").gameObject;
            
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
					Color tempColor;
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
						SetIndicatorColor(Indicator, tempColor);
						
					}
					if (System.Text.RegularExpressions.Regex.Match(Indicator.name, @"Input").Success)
					{
						if (hasSecondInput)
						{
							tempColor = Color.yellow;
						}
						else
						{
							tempColor = Color.black;								
						}	
						SetIndicatorColor(Indicator, tempColor);						
					}
					if (Indicator.name == "IndOutput")
					{
						if (hasSecondInput && BlockIsPowered)
						{
							tempColor = Color.blue;
						}
						else
						{
							tempColor = Color.black;								
						}	
						SetIndicatorColor(Indicator, tempColor);						
					}
				}
			}
<<<<<<< HEAD
		}
		
        DebugMsg("XR7");
		
=======
		}		
>>>>>>> 40d3c5c5f734ea212ca9546317df2e32453eacc7
		return true;
	}
	
	static Vector3i[] PowerInputLocations(Vector3i _blockPos)
	{
		int inputSpace = 1;
		Vector3i inputPosA = _blockPos;
		Vector3i inputPosB = _blockPos;
		Vector3i inputPosC = _blockPos;
		Vector3i inputPosD = _blockPos;
		Vector3i inputPosE = _blockPos;
		Vector3i inputPosF = _blockPos;
		
		inputPosA.y = _blockPos.y+inputSpace;
		inputPosB.y = _blockPos.y-inputSpace;
		inputPosC.x = _blockPos.x+inputSpace;
		inputPosD.x = _blockPos.x-inputSpace;
		inputPosE.z = _blockPos.z+inputSpace;
		inputPosF.z = _blockPos.z-inputSpace;
		
		Vector3i[] array = new Vector3i[6];
		array[0] = inputPosA;
		array[1] = inputPosB;
		array[2] = inputPosC;
		array[3] = inputPosD;
		array[4] = inputPosE;
		array[5] = inputPosF;
		return array;		
	}
	
	public static bool HasActivePower(WorldBase _world, int _cIdx, Vector3i _blockPos)
	{
		Vector3i[] locations = PowerInputLocations(_blockPos);
		foreach (Vector3i vector in locations)
		{
			BlockValue inputBlockValue = _world.GetBlock(vector);
			Type inputBlockType = Block.list[inputBlockValue.type].GetType();
			if(inputBlockType == typeof(BlockPowered))
			{
				TileEntityPowered tileEntityPowered = (TileEntityPowered)_world.GetTileEntity(_cIdx, vector);
				if (tileEntityPowered != null)
				{
					if(tileEntityPowered.IsPowered)
					{
						return true;
					}
				}
			}
		}
		return false;
	}
	

	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
        DebugMsg("OnBlockEntityTransformAfterActivated");
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		/*
 			bool flag = (_blockValue.meta & 1) != 0;
			bool flag2 = (_blockValue.meta & 2) != 0;
			if (_ebcd != null && _ebcd.transform != null && _ebcd.transform.gameObject != null)
			{
				Renderer[] componentsInChildren = _ebcd.transform.gameObject.GetComponentsInChildren<Renderer>();
				if (componentsInChildren != null)
				{
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						if (componentsInChildren[i].material != componentsInChildren[i].sharedMaterial)
						{
							componentsInChildren[i].material = new Material(componentsInChildren[i].sharedMaterial);
						}
						if (flag)
						{
							componentsInChildren[i].material.SetColor("_EmissionColor", (!flag2) ? Color.red : Color.green);
						}
						else
						{
							componentsInChildren[i].material.SetColor("_EmissionColor", Color.black);
						}
						componentsInChildren[i].sharedMaterial = componentsInChildren[i].material;
					}
				}
			}
		*/
	}

	public override bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
        DebugMsg("ActivateBlock");
		
		// Get call stack
		StackTrace stackTrace = new StackTrace();

		// Get calling method name
		DebugMsg(String.Concat("ActivateBlock Caller ", stackTrace.GetFrame(1).GetMethod()));
		DebugMsg(String.Concat("ActivateBlock Caller Caller ", stackTrace.GetFrame(2).GetMethod()));
		
		DebugMsg(String.Concat("ActivateBlock isPowered=", isPowered ? "1" : "0"));
		DebugMsg(String.Concat("ActivateBlock isOn=", isOn ? "1" : "0"));
		
		bool hasSecondInput = HasActivePower(_world, _cIdx, _blockPos);
		
		if(hasSecondInput && isPowered){
			// switch has 2nd input
			hasSecondInput = true;
		}
		else
		{
			hasSecondInput = false;
		}		
		DebugMsg(String.Concat("ActivateBlock hasSecondInput=", hasSecondInput ? "1" : "0"));
		

        TileEntityPoweredTrigger tileEntityPoweredTrigger = _world.GetTileEntity(_cIdx, _blockPos) as 	TileEntityPoweredTrigger;
        if (tileEntityPoweredTrigger != null)
        {
            if(isOn != hasSecondInput)
            {
            tileEntityPoweredTrigger.IsTriggered = hasSecondInput;//tileEntityPoweredTrigger.
            
            //tileEntityPoweredTrigger.Activate(isPowered, hasSecondInput);
            }
            DebugMsg(String.Concat("ActivateBlock tileEntityPoweredTrigger.IsTriggered=", tileEntityPoweredTrigger.IsTriggered ? "1" : "0"));
        }				
					

		_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | ((!hasSecondInput) ? 0 : 2));
		_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | ((!isPowered) ? 0 : 1));
		this.XR(_world, _cIdx, _blockPos, _blockValue, false);
		_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		return true;
	}

	public override void OnBlockValueChanged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
        DebugMsg("OnBlockValueChanged");
		base.OnBlockValueChanged(_world, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		this.XR(_world, _clrIdx, _blockPos, _newBlockValue, false);
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
		return new TileEntityPoweredTrigger(chunk)
		{
			TriggerType = PowerTrigger.TriggerTypes.Switch
		};
    } 

	public static bool IsSwitchOn(byte _metadata)
	{
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
	
	private void SetIndicatorColor(GameObject _obj, Color color)
	{
		Renderer rend =_obj.GetComponentInChildren<Renderer>();
		//rend.EnableKeyword("_EMISSION");
		rend.material.SetColor("_Emission", color);
	}
}
