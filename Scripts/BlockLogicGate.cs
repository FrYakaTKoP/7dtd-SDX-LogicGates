using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class BlockLogicGate : BlockPowered
{
	private bool showDebugLog = true;
	
	public void DebugMsg(string msg)
	{
		if(showDebugLog)
		{
			Debug.Log(msg);
		}
	}
	
    // get called at Gamestartup
    public override void Init()
	{
		base.Init();  
		// TODO: write fancy "mod Loaded" log		
		DebugMsg("BlockLogicGate.init");
	}

	
	// get called on first wire hookup  
    // doesn't care if wire is powered (even from a switch without powersource)
	// BlockPowered
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		DebugMsg("BlockLogicGate.CreateTileEntity");
		return new TileEntityPoweredBlock(chunk);
	}
	/*
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		DebugMsg("BlockLogicGate.CreateTileEntity");
	    return new TileEntityPoweredBlock(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.Consumer
        };
	}
	*/

	
	// Gets Called on lc pickup or on destroy/fall
	// BlockPowered
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		DebugMsg("BlockLogicGate.onBlockRemoved");
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		if (_blockValue.ischild)
		{
			DebugMsg("BlockLogicGate.onBlockRemoved _blockValue.ischild");
			while (true)
			{
				switch (7)
				{
				case 0:
					continue;
				}
				break;
			}
			if (!true)
			{
				// methodof error
				//RuntimeMethodHandle arg_26_0 = methodof(BlockPowered.OnBlockRemoved(WorldBase, Chunk, Vector3i, BlockValue)).MethodHandle;
			}
			return;
		}
		TileEntityPowered tileEntityPowered = _chunk.GetTileEntity(World.toBlock(_blockPos)) as TileEntityPowered;
		// don't know why but tileEntityPowered is null here but shouldn't 
		if (tileEntityPowered != null)
		{
			// not executed even when powered
			DebugMsg("BlockLogicGate.onBlockRemoved tileEntityPowered != null");
			while (true)
			{
				switch (2)
				{
				case 0:
					continue;
				}
				break;
			}
			if (!GameManager.IsDedicatedServer)
			{
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				if (primaryPlayer.inventory.holdingItem.Actions[1] is ItemActionConnectPower)
				{
					while (true)
					{
						switch (4)
						{
						case 0:
							continue;
						}
						break;
					}
					// GX is private -> see ItemActionConnectPower
					//(primaryPlayer.inventory.holdingItem.Actions[1] as ItemActionConnectPower).GX(primaryPlayer, _blockPos);
				}
			}
			if (Steam.Network.IsServer)
			{
				while (true)
				{
					switch (1)
					{
					case 0:
						continue;
					}
					break;
				}
				PowerManager.Instance.RemovePowerNode(tileEntityPowered.GetPowerItem());
			}
			if (tileEntityPowered.GetParent().y != -9999)
			{
				DebugMsg("BlockLogicGate.onBlockRemoved tileEntityPowered.GetParent().y != -9999");
				while (true)
				{
					switch (5)
					{
					case 0:
						continue;
					}
					break;
				}
				IPowered powered = world.GetTileEntity(0, tileEntityPowered.GetParent()) as IPowered;
				if (powered != null)
				{
					while (true)
					{
						switch (5)
						{
						case 0:
							continue;
						}
						break;
					}
					if (Steam.Network.IsServer)
					{
						while (true)
						{
							switch (5)
							{
							case 0:
								continue;
							}
							break;
						}
						powered.SendWireData();
					}
				}
			}
			tileEntityPowered.RemoveWires();
		}
		_chunk.RemoveTileEntityAt<TileEntityPowered>((World)world, World.toBlock(_blockPos));
	}
    
    // don't get called
    public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);        
		DebugMsg("BlockLogicGate.onBlockLoaded");
	}  
	

    
	// don't get called 
    public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);        
		DebugMsg("BlockLogicGate.OnBlockEntityTransformAfterActivated");
		if (_blockValue.ischild)
		{
			while (true)
			{
				switch (5)
				{
				case 0:
					continue;
				}
				break;
			}
			if (!true)
			{
                // trows Compiler Error CS1525
				//RuntimeMethodHandle arg_2A_0 = methodof(BlockPowered.OnBlockEntityTransformAfterActivated(WorldBase, Vector3i, int, BlockValue, BlockEntityData)).MethodHandle;
				
			}
			return;
		}

		TileEntityPowered tileEntityPowered = (TileEntityPowered)_world.GetTileEntity(_cIdx, _blockPos);
		if (tileEntityPowered != null)
		{
			tileEntityPowered.BlockTransform = _ebcd.transform;
			//GameManager.Instance.StartCoroutine(this.OE(tileEntityPowered));
			if (tileEntityPowered.GetParent().y != -9999)
			{
				while (true)
				{
					switch (3)
					{
					case 0:
						continue;
					}
					break;
				}
				IPowered powered = _world.GetTileEntity(0, tileEntityPowered.GetParent()) as IPowered;
				if (powered != null)
				{
					DebugMsg("BlockLogicGates.OnBlockEntityTransformAfterActivated powered != null ");
					//GameManager.Instance.StartCoroutine(this.OE(powered));
				}
			}
		}

	}
	
	// don't get called
	public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		this.shape.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		DebugMsg("BlockLogicGate.OnBlockEntityTransformBeforeActivated");
	}
	
	/* 
    // throws error CS0305 
    // BlockPowered
    private IEnumerator OE(IPowered powered)
	{
		BlockPowered.DT dT = new BlockPowered.DT();
		dT.BU = powered;
		dT.AU = powered;
		return dT;
	}

	
	public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		this.shape.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		GameObject gameObject = _ebcd.transform.gameObject;
		LogicGates script = gameObject.GetComponent<LogicGates>();
		if(script == null)
		{
			script = gameObject.AddComponent<LogicGates>();
		}
		script.enabled = true;
		script.blockPos = _blockPos;
		script.cIdx = _cIdx;
		script.ebcd = _ebcd;
		DebugMsg("BlockLogicGate.OnBlockEntityTransformBeforeActivated");
	}	
   */
}