using System;
using UnityEngine;
using System.Collections.Generic;

public class LogicGateController : MonoBehaviour 
{
	public int cIdx;
	public Vector3i blockPos;
	public GameObject litSignObject;
	private bool isMainPower;
	private bool flash;
	
	private static List<GameObject> Indicators;
	
	private static  bool showDebugLog = true;

	public static void DebugMsg(string msg)
	{
		if(showDebugLog)
		{
            String msg2 = String.Concat("SDX LogicGatesControl.", msg);
			Debug.Log(msg2);
		}
	}
	
	void Awake()
	{
		BlockValue blockValue = GameManager.Instance.World.GetBlock(blockPos);
		Block block = Block.list[blockValue.type];
		if (block.Properties.Values.ContainsKey("Flash"))
        {
			bool.TryParse(block.Properties.Values["Flash"], out flash);
        }
		else
		{
			flash = false;
		}
		GetColorSlots(block);
		litSignObject.active = true;
	}
	
	void Update()
	{
		string color =  "";
		
		if(BlockLogicGate.isBlockPoweredUp(blockPos, cIdx))
		{
			isMainPower = true;
		}
		else
		{
			isMainPower = false;
		}
		if(isMainPower)
		{		
			color = "0, 138, 0";
		}
		else
		{
			color = "0, 0, 0";
		}
		if(Indicators != null)
		{
			foreach (GameObject Indicator in Indicators)
			{
			if (Indicator.name == "IndMainPower")
				{
					Vector3 colorVector = Utils.ParseVector3(color);
					SetColor(Indicator, colorVector);
				}
			}			
		}
		else
		{			
			//DebugMsg("Update Indicators == null");
		}
		return;
	}
	
	private void GetColorSlots(Block block)
	{
		Transform[] colorSlots = litSignObject.GetComponentsInChildren<Transform>();
		foreach (Transform slot in colorSlots)
		{
			GameObject slotObject = slot.gameObject;
			if(slotObject.name.Contains("Ind"))
			{		
				if(Indicators == null)
				{
					DebugMsg("GetColorSlots Indicators == null");
					Indicators = new List<GameObject>();
				}
				Indicators.Add( slotObject );
			}
		}
	}
	
	private void SetColor(GameObject _slotObject, Vector3 _colorVector)
	{
		Color color = new Color(_colorVector.x, _colorVector.y, _colorVector.z);
		Transform[] children = _slotObject.GetComponentsInChildren<Transform>();
		foreach (Transform child in children)
		{
			GameObject gameObject = child.gameObject;
			Renderer rend = gameObject.GetComponent<Renderer>();
			if(rend != null)
			{
				rend.material.EnableKeyword("_EMISSION");
				rend.material.SetColor("_Emission", color);
			}
		}
	}
}