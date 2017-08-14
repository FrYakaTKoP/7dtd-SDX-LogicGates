using System;
using UnityEngine;


public class LogicGates : MonoBehaviour
{
	public Vector3i blockPos;
	public int cIdx;
	public BlockEntityData ebcd;
	public TileEntity parentTileEntity;
	public TileEntityPowerSource parentPowerSource;
	int count = 0;
	int passCount = 0;
	private bool showDebugLog = true;
	
	public void DebugMsg(string msg)
	{
		if(showDebugLog)
		{
			Debug.Log(msg);
		}
	}
	
	void Update()
	{
		if(IsConnectedToPowerSource())
		{
			GetPowerSource();
		}
	}
	
	void GetPowerSource()
	{
		TileEntity tileEntity = GameManager.Instance.World.GetTileEntity(cIdx, blockPos);
		TileEntityPowerSource tileEntityPowerSource = (TileEntityPowerSource)tileEntity;
		PowerSource powerSource = tileEntityPowerSource.GetPowerItem() as PowerSource;
		if (powerSource == null)
		{
			powerSource = (PowerManager.Instance.GetPowerItemByWorldPos(tileEntityPowerSource.ToWorldPos()) as PowerSource);
			
		}
		if (powerSource != null)
		{
			DebugMsg("got powerSource");
			ushort maxPower = tileEntityPowerSource.MaxOutput;
			DebugMsg("maxPower: " + maxPower);
			PowerItem parentPowerItem = powerSource.Parent;
			if(parentPowerItem == null)
			{
				//Debug.Log("No parent found.");
				return;
			}
			Vector3i parentPos = powerSource.Parent.Position;
			if(parentPos != null)
			{
				//Debug.Log("Parent Position: " + parentPos);
				parentTileEntity = GameManager.Instance.World.GetTileEntity(cIdx, parentPos);
				if(parentTileEntity != null)
				{
					parentPowerSource = (TileEntityPowerSource)parentTileEntity;
					if(parentPowerSource != null)
					{

					}
					else
					{
						DebugMsg("Parent tileEntity is not a power source");
						return;
					}
				}
			}
			else
			DebugMsg("parentPos null");
		}
		else
		DebugMsg("powerSource null");
	}
	
	public bool IsConnectedToPowerSource()
	{
		TileEntity tileEntity = GameManager.Instance.World.GetTileEntity(cIdx, blockPos);
		TileEntityPowerSource tileEntityPowerSource = (TileEntityPowerSource)tileEntity;
		PowerSource powerSource = tileEntityPowerSource.GetPowerItem() as PowerSource;
		if (powerSource == null)
		{
			powerSource = (PowerManager.Instance.GetPowerItemByWorldPos(tileEntityPowerSource.ToWorldPos()) as PowerSource);
			
		}
		PowerItem parentPowerItem = powerSource.Parent;
		if(parentPowerItem == null)
		{
			return false;
		}
		WorldBase worldBase = GameManager.Instance.World;
		Vector3i parentPos = powerSource.Parent.Position;
		BlockValue blockValue = worldBase.GetBlock(parentPos);
		Block block = Block.list[blockValue.type];
		Type parentBlockType = Block.list[blockValue.type].GetType();
		if(parentBlockType == typeof(BlockGenerator))
		{
			parentTileEntity = GameManager.Instance.World.GetTileEntity(cIdx, parentPos);
			if(parentTileEntity != null)
			{
				parentPowerSource = (TileEntityPowerSource)parentTileEntity;
				PowerSource parentPower = parentPowerSource.GetPowerItem() as PowerSource;
				PowerGenerator powerGenerator = parentPower as PowerGenerator;
				ushort maxPowerOutput = parentPower.MaxOutput;
				if(maxPowerOutput >= 1)
				{
					if(powerGenerator.CurrentFuel >= 1)
					{
						return true;
					}
				}
			}
		}
		if(parentBlockType == typeof(BlockSolarPanel))
		{
			return true;
		}
		return false;
	}
	
	public bool HasWorkingSlotItem(PowerSource _parentPower)
	{
		int slotCount = _parentPower.SlotCount;
		if(slotCount == 0)
		{
			//Debug.Log("No Slot items found, can not activate power source.");
			return false;
		}
		foreach (ItemStack stack in _parentPower.Stacks)
		{
			ItemValue itemValue = stack.itemValue;
			int maxUseTimes = itemValue.MaxUseTimes;
			int useTimes = itemValue.UseTimes;
			if(useTimes <= maxUseTimes)
			{
				//Debug.Log("Parent Power Source has atleast one working slot item.");
				return true;
			}
		}
		//Debug.Log("All slot items are broken, can not activate power source.");
		return false;
	}
}