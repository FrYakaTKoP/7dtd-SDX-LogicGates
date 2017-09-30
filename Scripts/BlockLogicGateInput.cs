using GUI_2;
using System;
using UnityEngine;

using System.Diagnostics;

public class BlockLogicGateInput : BlockPowered
{
	private BlockEntityData HT;

	private Vector3 XT;

	private bool RuntimeSwitch;

	private BlockActivationCommand[] MU = new BlockActivationCommand[]
	{
		new BlockActivationCommand("light", "electric_switch", true),
		new BlockActivationCommand("take", "hand", false)
	};
	
	private bool showDebugLog = true;

	public void DebugMsg(string msg)
	{
		if(showDebugLog)
		{
            String msg2 = String.Concat("SDX BlockLogicGateInput.", msg);
			UnityEngine.Debug.Log(msg2);
		}
	}

	public override void Init()
	{
		base.Init();
		this.RuntimeSwitch = true;
		/*	
			// BLockLight
			if (this.Properties.Values.ContainsKey("RuntimeSwitch"))
			{
				this.RuntimeSwitch = bool.Parse(this.Properties.Values["RuntimeSwitch"]);
			}
		*/
	}
	
/* 
	public override byte GetLightValue(BlockValue _blockValue)
	{
		if ((_blockValue.meta & 2) == 0)
		{
			return 0;
		}
		return base.GetLightValue(_blockValue);
	}
 */
 
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.IsEditor())
		{
			if (this.RuntimeSwitch)
			{
				TileEntityPoweredBlock tileEntityPoweredBlock = (TileEntityPoweredBlock)_world.GetTileEntity(_clrIdx, _blockPos);
				if (tileEntityPoweredBlock != null)
				{
					PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
					string keybindString = UIUtils.GetKeybindString(playerInput.Activate, playerInput.PermanentActions.Activate);
					if (tileEntityPoweredBlock.IsToggled)
					{
						//return string.Format(Localization.Get("useSwitchLightOff", string.Empty), keybindString);
						return string.Format("<{0}> toggle ActiveLow", keybindString);
					}
					//return string.Format(Localization.Get("useSwitchLightOn", string.Empty), keybindString);
					return string.Format("<{0}> toggle ActiveHigh", keybindString);
				}
			}
			else if (this.MU[1].enabled)
			{
				Block block = Block.list[_blockValue.type];
				string blockName = block.GetBlockName();
				return string.Format(Localization.Get("pickupPrompt", string.Empty), Localization.Get(blockName, string.Empty));
			}
			return null;
		}
		PlayerActionsLocal playerInput2 = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string keybindString2 = UIUtils.GetKeybindString(playerInput2.Activate, playerInput2.PermanentActions.Activate);
		if ((_blockValue.meta & 2) != 0)
		{
			//return string.Format(Localization.Get("useSwitchLightOff", string.Empty), keybindString2);
			return string.Format("<{0}> toggle ActiveLow", keybindString2);
		}
		//return string.Format(Localization.Get("useSwitchLightOn", string.Empty), keybindString2);
		return string.Format("<{0}> toggle ActiveHigh", keybindString2);
	}

	public override bool OnBlockActivated(int _indexInBlockActivationCommands, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _player)
	{		
        DebugMsg("OnBlockActivated");  
        
        
		if (_indexInBlockActivationCommands != 0)
		{
			if (_indexInBlockActivationCommands == 1)
			{
				base.TakeItemWithTimer(_cIdx, _blockPos, _blockValue, _player);
				return true;
			}
		}
		else if (_world.IsEditor())
		{
			if (this.ZR(_world, _cIdx, _blockPos, _blockValue, true))
			{
				return true;
			}
		}
		else
		{
			TileEntityPoweredBlock tileEntityPoweredBlock = (TileEntityPoweredBlock)_world.GetTileEntity(_cIdx, _blockPos);
			if (tileEntityPoweredBlock != null)
			{
				tileEntityPoweredBlock.IsToggled = !tileEntityPoweredBlock.IsToggled;
				DebugMsg(String.Concat("IsToggled changed =", tileEntityPoweredBlock.IsToggled));
			}
		}
		return false;
	}
	

	private bool ZR(WorldBase worldBase, int num, Vector3i blockPos, BlockValue blockValue, bool flagOnBlockActivatedIsEditor)
	{
        DebugMsg("XR");		
		 if(flagOnBlockActivatedIsEditor == null)
        {
            flagOnBlockActivatedIsEditor = false;
        }
		ChunkCluster chunkCluster = worldBase.ChunkClusters[num];
		if (chunkCluster == null)
		{
			return false;
		}
		IChunk chunkSync = chunkCluster.GetChunkSync(World.toChunkXZ(blockPos.x), World.toChunkY(blockPos.y), World.toChunkXZ(blockPos.z));
		if (chunkSync == null)
		{
			return false;
		}
		BlockEntityData blockEntity = chunkSync.GetBlockEntity(blockPos);
		if (blockEntity != null && blockEntity.bHasTransform)
		{
			bool flag2 = (blockValue.meta & 2) != 0; // blockValueMetaIsToggled
			TileEntityPoweredBlock tileEntityPoweredBlock = (TileEntityPoweredBlock)worldBase.GetTileEntity(num, blockPos);
			if (tileEntityPoweredBlock != null)
			{
				flag2 = (flag2 && tileEntityPoweredBlock.IsToggled);
			}
			if (flagOnBlockActivatedIsEditor)
			{
				flag2 = !flag2;
				blockValue.meta = (byte)(((int)blockValue.meta & -3) | ((!flag2) ? 0 : 2));
				worldBase.SetBlockRPC(num, blockPos, blockValue);
			}
			return true;
		}
		return false;
	}

	public override void OnBlockValueChanged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		this.ZR(_world, _clrIdx, _blockPos, _newBlockValue, false);
	}

	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer());
		this.MU[0].enabled = (_world.IsEditor() || this.RuntimeSwitch);
		this.MU[1].enabled = (flag && this.TakeDelay > 0f);
		return this.MU;
	}

	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		this.ZR(_world, _cIdx, _blockPos, _blockValue, false);
	}

	public override bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
        DebugMsg("ActivateBlock");
		
		/* 
			StackTrace st = new StackTrace(1, true);
			StackFrame [] stFrames = st.GetFrames();
			for(int i=0; i < stFrames.Length; i++ )
			{
			   DebugMsg(String.Concat("ActivateBlock CallingMethod:",stFrames[i].GetMethod() ));
			}
		*/	
		
		bool BlockIsToggled = (_blockValue.meta & 2) != 0;

		DebugMsg(String.Concat("ActivateBlock isPowered=", isPowered ? "true" : "false"));
		DebugMsg(String.Concat("ActivateBlock isOn=", isOn ? "true" : "false"));
		
		BlockEntityData _ebcd = ((World)_world).ChunkClusters[_cIdx].GetBlockEntity(_blockPos);
		if (_ebcd != null && _ebcd.transform != null && _ebcd.transform.gameObject != null)
		{	
			// foreach( Transform trans in _ebcd.transform) 
			// {
				// DebugMsg(String.Concat("ActivateBlock parent.transform.name=", trans.name ));
			// }
			//GameObject IndicatorsObj = _ebcd.transform.Find("Position").gameObject;			
			GameObject IndicatorsObj = _ebcd.transform.Find("WireOffset").gameObject;			
			
			if(IndicatorsObj == null)
			{
				DebugMsg("IndicatorsObj is null");
			}
			Color tempColor = Color.black;
						
			if (isPowered)
			{
				if(BlockIsToggled)
				{
					tempColor = Color.green;
				}
				else
				{
					tempColor = Color.red;
				}
			}
			else
			{
				if(BlockIsToggled)
				{
					tempColor = Color.black;
				}
				else
				{
					tempColor = Color.yellow;
				}
			}	
			SetIndicatorColor(IndicatorsObj, tempColor);
			
			// Transform[] Indicators = IndicatorsObj.GetComponentsInChildren<Transform>();
			// if (Indicators != null)
			// {
				// GameObject Indicator = Indicators[0].gameObject;;
				// Color tempColor = Color.black;
				// for (int i = 0; i < Indicators.Length; i++)
				// {
					// DebugMsg(String.Concat("ActivateBlock Indicator.name=", Indicator.name ));
					// Indicator = Indicators[i].gameObject;					
					// if (Indicator.name == "LOD0")
					// {
						// DebugMsg("ActivateBlock found LOD0");
						// if (isPowered)
						// {
							// DebugMsg("ActivateBlock tempColor = Color.green;");
							// tempColor = Color.green;
						// }
						// else
						// {
							// tempColor = Color.black;								
						// }	
					// }
				// }
				// SetIndicatorColor(Indicator, tempColor);
			// }
		}
		
        
		_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | ((!isOn) ? 0 : 2));
		_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		this.ZR(_world, _cIdx, _blockPos, _blockValue, false);
		return true;
	}
	
	

	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		PowerItem.PowerItemTypes powerItemType = PowerItem.PowerItemTypes.Consumer;
		if (this.RuntimeSwitch)
		{
			powerItemType = PowerItem.PowerItemTypes.ConsumerToggle;
		}
		return new TileEntityPoweredBlock(chunk)
		{
			PowerItemType = powerItemType
		};
	}
	
	private void SetIndicatorColor(GameObject _obj, Color color)
	{
		//DebugMsg(String.Concat("SetIndicatorColor =", ()? "1" : "0")); 
				
		Renderer rend =_obj.GetComponentInChildren<Renderer>();
		
		// string[] kws = rend.material.shaderKeywords;
		// foreach ( string keyword in kws)
		// {
			// DebugMsg(String.Concat("SetIndicatorColor renderer.material. keyword:", keyword)); 
		// }
		
		// DebugMsg(String.Concat("SetIndicatorColor renderer.material.name=", rend.material.name ));
		// DebugMsg(String.Concat("SetIndicatorColor renderer.material.shader.name=", rend.material.shader.name ));		
		//DebugMsg(String.Concat("SetIndicatorColor renderer.material. hasProp:_EmissionColor=", rend.material.HasProperty("_EmissionColor") ? "1" : "0" )); // true
		//DebugMsg(String.Concat("SetIndicatorColor renderer.material. hasProp:_Color=", rend.material.HasProperty("_Color") ? "1" : "0" )); // true
		//DebugMsg(String.Concat("SetIndicatorColor renderer.material. hasProp:_SpecColor=", rend.material.HasProperty("_SpecColor") ? "1" : "0" )); // true

		
		Light li =_obj.GetComponentInChildren<Light>();
		if(li == null )
		{
			li = _obj.AddComponent<Light>();
			li.range = 1;
			li.intensity = 1;
		}
		if(color == Color.green)
		{
			//rend.material.mainTexture = null;				
			li.enabled = true;
		}
		else
		{				
			li.enabled = false;
		}
		li.color = color;
		


	}
}
