using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Exiled.API.Features;
using MapEditorReborn.API.Extensions;
using UnityEngine;

namespace PlayhousePlugin.Components
{
	public class PlayhousePluginComponent : MonoBehaviour
	{
		public Player player { get; private set; }
		
		private string _hudTemplate = "<line-height=95%><voffset=8.5em><align=left><size=50%><alpha=#44>[STATS]<alpha=#ff></size></align>\n<align=right>[LIST]</align><align=center>[CENTER_UP][CENTER][CENTER_DOWN][BOTTOM]";
		private float _timer = 0f;
		private Tip _proTip;
		private int _timerCount = 0;
		private string _hudText = string.Empty;
		
		private string _hudCenterUpString = string.Empty;
		private float _hudCenterUpTime = -1f;
		private float _hudCenterUpTimer = 0f;
		
		private string _hudCenterString = string.Empty;
		private float _hudCenterTime = -1f;
		private float _hudCenterTimer = 0f;
		
		private string _hudCenterDownString = string.Empty;
		private float _hudCenterDownTime = -1f;
		private float _hudCenterDownTimer = 0f;
		
		public static List<PlayhousePluginComponent> Instances = new List<PlayhousePluginComponent>();
		
		//private string _hudBottomDownString = string.Empty;
		//private float _hudBottomDownTime = -1f;
		//private float _hudBottomDownTimer = 0f;

		private void Start()
		{
			player = Player.Get(gameObject);
			Instances.Add(this);
			//_hudTemplate = _hudTemplate.Replace("[VERSION]", $"Ver{}");
		}

		private void FixedUpdate()
		{
			_timer += Time.deltaTime;

			UpdateTimers();
			//CheckVoiceChatting();				

			//EverySeconds
			if(_timer > 0.5f)
			{
				//CheckSinkholeDistance();
				//Check079Spot();

				//UpdateScpLists();
				//UpdateMyCustomText();
				//UpdateRespawnCounter();

				if (player.CurrentItem != null)
				{
					if(!player.CurrentItem.IsToolGun())
						UpdateExHud();
				}
				else
				{
					UpdateExHud();
				}
				
				_timer = 0f;
			}				
		}

		public void AddHudCenterUpText(string text, ulong timer)
		{
			_hudCenterUpString = text;
			_hudCenterUpTime = timer;
			_hudCenterUpTimer = 0f;
		}

		public void ClearHudCenterUpText()
		{
			_hudCenterTime = -1f;
		}
		
		public void AddHudCenterText(string text, ulong timer)
		{
			_hudCenterString = text;
			_hudCenterTime = timer;
			_hudCenterTimer = 0f;
		}

		public void ClearHudCenterText()
		{
			_hudCenterTime = -1f;
		}
		
		public void AddHudCenterDownText(string text, ulong timer)
		{
			_hudCenterDownString = text;
			_hudCenterDownTime = timer;
			_hudCenterDownTimer = 0f;
		}

		public void ClearHudCenterDownText()
		{
			_hudCenterDownTime = -1f;
		}
		
		/*
		public void AddHudBottomText(string text, ulong timer)
		{
			_hudBottomDownString = text;
			_hudBottomDownTime = timer;
			_hudBottomDownTimer = 0f;
		}

		public void ClearHudBottomText()
		{
			_hudBottomDownTime = -1f;
		}*/

		public void UpdateTimers()
		{
			if (_hudCenterUpTimer < _hudCenterUpTime)
				_hudCenterUpTimer += Time.deltaTime;
			else
				_hudCenterUpString = string.Empty;
			
			if (_hudCenterTimer < _hudCenterTime)
				_hudCenterTimer += Time.deltaTime;
			else
				_hudCenterString = string.Empty;
			
			if(_hudCenterDownTimer < _hudCenterDownTime)
				_hudCenterDownTimer += Time.deltaTime;
			else
				_hudCenterDownString = string.Empty;
			
			/*
			if(_hudBottomDownTimer < _hudBottomDownTime)
				_hudBottomDownTimer += Time.deltaTime;
			else
				_hudBottomDownString = string.Empty;*/
		}

		private void UpdateExHud()
		{
			string curText = _hudTemplate.Replace("[STATS]",
				$"<color=#FF0000>K</color><color=#FF5500>o</color><color=#FFAA00>g</color><color=#FFFF00>n</color><color=#CCFF00>i</color><color=#99FF00>t</color><color=#66FF00>y</color><color=#33FF00>'</color><color=#00FF00>s</color><color=#00FF3F> </color><color=#00FF7F>P</color><color=#00FFBF>l</color><color=#00FFFF>a</color><color=#00CCFF>y</color><color=#0099FF>h</color><color=#0066FF>o</color><color=#0033FF>u</color><color=#0000FF>s</color><color=#3F00FF>e</color><color=#7F00FF> </color><color=#BF00FF>{Utils.ServerPort[Server.Port]}</color> [Server Time: {DateTime.Now:HH:mm:ss}]");

			// The top left "SCP LIST" thing
			//curText = curText.Replace("[LIST]", FormatStringForHud(string.Empty, 7));
			
			//[LIST]
			if(AlphaWarheadController.Host.inProgress && !AlphaWarheadController.Host.detonated && !RoundSummary.singleton.RoundEnded)
			{
				int TargettMinus = AlphaWarheadController._resumeScenario == -1
					? AlphaWarheadController.Host.scenarios_start[AlphaWarheadController._startScenario].tMinusTime
					: AlphaWarheadController.Host.scenarios_resume[AlphaWarheadController._resumeScenario].tMinusTime;

				if(!UtilityMethods.IsAlphaWarheadCountdown())
					curText = curText.Replace("[LIST]", FormatStringForHud($"\n{TargettMinus / 60:00} : {TargettMinus % 60:00}", 7));
				else
					curText = curText.Replace("[LIST]", FormatStringForHud($"<color=#ff0000>\n{Mathf.FloorToInt(AlphaWarheadController.Host.timeToDetonation) / 60:00} : {Mathf.FloorToInt(AlphaWarheadController.Host.timeToDetonation) % 60:00}</color>", 7));
			}
			else
				curText = curText.Replace("[LIST]", FormatStringForHud(string.Empty, 6));
			
			//[CENTER_UP]
			if(!string.IsNullOrEmpty(_hudCenterUpString))
				curText = curText.Replace("[CENTER_UP]", FormatStringForHud(_hudCenterUpString, 6));
			else
				curText = curText.Replace("[CENTER_UP]", FormatStringForHud(string.Empty, 6));

			//[CENTER]
			if(!string.IsNullOrEmpty(_hudCenterString))
				curText = curText.Replace("[CENTER]", FormatStringForHud(_hudCenterString, 6));
			else
				curText = curText.Replace("[CENTER]", FormatStringForHud(string.Empty, 6));

			//[CENTER_DOWN]
			if(!string.IsNullOrEmpty(_hudCenterDownString))
				curText = curText.Replace("[CENTER_DOWN]", FormatStringForHud(_hudCenterDownString, 7));
			else
				curText = curText.Replace("[CENTER_DOWN]", FormatStringForHud(string.Empty, 7));

			//[BOTTOM]
			/*
			if(AbilitySelection != Utils.Ability.None)
				curText = curText.Replace("[BOTTOM]", $"Selected: {AbilitySelection.ToString()}");
			else
				curText = curText.Replace("[BOTTOM]", "　");*/

			if (player.IsAlive)
			{
				if (player.HasCurrentAbilitySelection())
				{
					curText = curText.Replace("[BOTTOM]",
						player.CustomClassManager().CustomClass
							.ActiveAbilities[player.CustomClassManager().AbilityIndex]
							.GenerateHud());
				}
				else
					curText = curText.Replace("[BOTTOM]", "　");
				
				_proTip = null;
				_timerCount = 0;
			}
			else
			{
				if (_proTip == null)
				{
					_proTip = Utils.Tips.PickRandom();
				}
				
				_timerCount++;
				if (_timerCount != 20)
				{
					curText = curText.Replace("[BOTTOM]", _proTip.Message);
				}
				else
				{
					_proTip = Utils.Tips.PickRandom();
					curText = curText.Replace("[BOTTOM]", _proTip.Message);
					_timerCount = 0;
				}
			}

			_hudText = curText;
			player.SendTextHintNotEffect(_hudText, 1);
		}

		private string FormatStringForHud(string text, int needNewLine)
		{
			int curNewLine = text.Count(x => x == '\n');
			for(int i = 0; i < needNewLine - curNewLine; i++)
				text += '\n';
			return text;
		}
	}
}