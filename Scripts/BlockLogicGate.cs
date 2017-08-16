using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class BlockLogicGate : BlockPowered
{
    // get called at Gamestartup
    public override void Init()
	{
		base.Init();        
		Debug.Log("BlockLogicGate.init");
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
	}
    
    // don't get called
    public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);        
		DebugMsg("BlockLogicGate.onBlockLoaded");
	}  
	    
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		Debug.Log("BlockLogicGate.CreateTileEntity");
        return new TileEntityPoweredBlock(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.Consumer
        };
	}
    // called on block placement
    // Block
    public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		Debug.Log("BlockLogicGate.OnBlockAdded");
        base.OnBlockAdded( _world, _chunk, _blockPos, _blockValue);
	}
    
    // called if a neighbor block gets added, removed (TODO: does this execute on changes like inventory of chests, etc?)
    // Block
    public override void OnNeighborBlockChange(WorldBase world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		Debug.Log("BlockLogicGate.OnNeighborBlockChange");
        base.OnNeighborBlockChange(world,_clrIdx, _myBlockPos, _myBlockValue, _blockPosThatChanged, _newNeighborBlockValue, _oldNeighborBlockValue);
	}
    
    // connecting or disconnecting does not fire this :( keep on searching...
    // block
    public override void OnBlockValueChanged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{    
		Debug.Log("BlockLogicGate.OnBlockValueChanged");
		base.OnBlockValueChanged(_world, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
	}
    
   /*
    public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);        
		Debug.Log("BlockLogicGate.OnBlockEntityTransformAfterActivated");
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
			GameManager.Instance.StartCoroutine(this.OE(tileEntityPowered));
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
					GameManager.Instance.StartCoroutine(this.OE(powered));
				}
			}
		}

	}
    
 
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
		Debug.Log("BlockLogicGate.OnBlockEntityTransformBeforeActivated");
	}	
   */
}