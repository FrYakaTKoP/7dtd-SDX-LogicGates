using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class BlockLogicGate : BlockPowered
{
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		Debug.Log("Powered");
		return new TileEntityPoweredBlock(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.Consumer
		};
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
		Debug.Log("LogicGates transform before act");
	}	
}