using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace VampireSurvivor
{
    public class GameUpgradePanel : MonoBehaviour
    {
        [SerializeField] private GameObject upgradePanel;

        [SerializeField] private List<OptionUI> optionUIList;

        [SerializeField] private List<UpgradeOption> upgradeOptions;

        private List<UpgradeOption> currentUpgradeOptions = new();

        private void OnEnable()
        {
            EventManager.StartListening(GameEvents.PlayerLevelUp, OnPlayerLevelUp);
        }

        private void SetUpUpgradePanel()
        {
            for (int i = 0; i < optionUIList.Count; i++)
            {
                OptionUI optionUI = optionUIList[i];
                IUpgradeOption upgradeOption = GetRandomOption();
                optionUI.SetOption(upgradeOption, () =>
                {
                    ToggleGamePauseState(false);
                    ToggleUpgradePanel(false);
                });
            }
        }

        private void ClearTheUpgradeOptions()
        {
            for (int i = 0; i < optionUIList.Count; i++)
            {
                OptionUI optionUI = optionUIList[i];
                optionUI.ClearTheUpgradeOption();
            }
            currentUpgradeOptions.Clear();
        }

        private IUpgradeOption GetRandomOption()
        {
            int randomNumber = Random.Range(0, optionUIList.Count);
            UpgradeOption upgradeOption = upgradeOptions[randomNumber];
            if (!currentUpgradeOptions.Contains(upgradeOption))
            {
                currentUpgradeOptions.Add(upgradeOption);
            }
            else
            {
                return GetRandomOption();
            }
            return upgradeOption;
        }

        private void OnDisable()
        {
            EventManager.StopListening(GameEvents.PlayerLevelUp, OnPlayerLevelUp);
        }

        private void OnPlayerLevelUp(object obj)
        {
            SetUpUpgradePanel();
            ToggleUpgradePanel(true);
            ToggleGamePauseState(true);
        }

        private void ToggleUpgradePanel(bool state)
        {
            upgradePanel.SetActive(state);
        }

        private void ToggleGamePauseState(bool state)
        {
            EventManager.TriggerEvent(GameEvents.GamePause, state);
        }
    }
}