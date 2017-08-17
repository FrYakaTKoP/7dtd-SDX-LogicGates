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
    
    // get called at Gamestartup (main menu)
    public override void Init()
	{
		base.Init();        
		DebugMsg("BlockLogicGate.init");
	}
				
    // called right after init()
    // Block
    public override void LateInit()
	{    
		DebugMsg("BlockLogicGate.LateInit");
        base.LateInit();
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
    
    // don't get called
	// BlockPowered
    public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		DebugMsg("BlockLogicGate.onBlockLoaded");
		base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);        
	}  
    

    // block
   	public override void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, System.Random _rnd)
	{
		DebugMsg("BlockLogicGate.OnBlockPlaceBefore");
		base.OnBlockPlaceBefore(_world, ref _bpResult, _ea, _rnd);
    }   
   
   
    // called on Block Placement
    // Block
    public virtual void PlaceBlock(WorldBase _world, ref BlockPlacement.Result _result, EntityAlive _ea)
	{
		DebugMsg("BlockLogicGate.PlaceBlock");
        base.PlaceBlock(_world, _result, _ea);
	}

    // called after Block was added
    // Block
    public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		DebugMsg("BlockLogicGate.OnBlockAdded");
        base.OnBlockAdded( _world, _chunk, _blockPos, _blockValue);
	}
	
  /*
	// blockPressurePlate
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
   
    // called if a neighbor block gets added, removed (TODO: does this execute on changes like inventory of chests, etc?)
    // Block
    public override void OnNeighborBlockChange(WorldBase world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		DebugMsg("BlockLogicGate.OnNeighborBlockChange");
        base.OnNeighborBlockChange(world,_clrIdx, _myBlockPos, _myBlockValue, _blockPosThatChanged, _newNeighborBlockValue, _oldNeighborBlockValue);
	}
    
    // connecting or disconnecting does not fire this :( keep on searching...
    // block
    public override void OnBlockValueChanged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{    
		DebugMsg("BlockLogicGate.OnBlockValueChanged");
		base.OnBlockValueChanged(_world, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
	}
   

   
   
    //
    // Block
    public virtual bool UpdateTick(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bRandomTick, ulong _ticksIfLoaded, System.Random _rnd)
	{
		DebugMsg("BlockLogicGate.UpdateTick");
		return false;
	}
    
    // Gets Called on LandClaimPickup or on destroy/fall
 	// BlockPowered
 	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
 	{
 		DebugMsg("BlockLogicGate.onBlockRemoved");
        TileEntityPowered tileEntityPowered = _chunk.GetTileEntity(World.toBlock(_blockPos)) as TileEntityPowered;
		if (tileEntityPowered != null)
		{
			if (tileEntityPowered.GetParent().y != -9999)
			{
				IPowered powered = world.GetTileEntity(0, tileEntityPowered.GetParent()) as IPowered;
				if (powered != null)
				{
                    DebugMsg("BlockLogicGate.OnBlockRemoved - powered != null"); 
				}
			}
		}
 		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
 	}

    // BlockPowered
	public override void OnBlockEntityTransformBeforeActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		DebugMsg("BlockLogicGate.OnBlockEntityTransformBeforeActivated");        
		base.OnBlockEntityTransformBeforeActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
	}
    
   // BlockPowered
    public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
        DebugMsg("BlockLogicGate.OnBlockEntityTransformAfterActivated");    
		TileEntityPowered tileEntityPowered = (TileEntityPowered)_world.GetTileEntity(_cIdx, _blockPos);
		if (tileEntityPowered != null)
		{
			if (tileEntityPowered.GetParent().y != -9999)
			{
				IPowered powered = _world.GetTileEntity(0, tileEntityPowered.GetParent()) as IPowered;
				if (powered != null)
				{
                    DebugMsg("BlockLogicGate.OnBlockEntityTransformAfterActivated - powered != null"); 
				}
			}
		}
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
	}
    
    //
    public override void DoExchangeAction(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, string _action, int _itemCount)
	{        
        DebugMsg("BlockLogicGate.DoExchangeAction"); 
	}
  
  /*  
	// called when Entity (player, z, etc.) Collide with Block 
	// BlockPressurePlate
	public override bool OnEntityCollidedWithBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, Entity _targetEntity)
	{
        DebugMsg("BlockLogicGate.OnEntityCollidedWithBlock"); 
		
		if (!(_targetEntity is EntityAlive))
		{
			return false;
		}
		EntityAlive entityAlive = (EntityAlive)_targetEntity;
		if (entityAlive.IsDead())
		{
			return false;
		}
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			_blockValue = _world.GetBlock(parentPos);
		}
		_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | 2);
		_world.SetBlockRPC(_clrIdx, _blockPos, _blockValue);
		return true;
	}
  */
	
  /*
        LogicGates script = gameObject.GetComponent<LogicGates>();
		if(script == null)
		{
			script = gameObject.AddComponent<LogicGates>();
		}
		script.enabled = true;
		script.blockPos = _blockPos;
		script.cIdx = _cIdx;
		script.ebcd = _ebcd;
   */
}