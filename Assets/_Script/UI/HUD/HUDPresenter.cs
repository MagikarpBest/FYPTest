using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HUD
{
    // TODO: Add skills list and subscribe to action input from HUDView.
    /// <summary>
    /// Owns Bind/Unbind (subscribing to whichever HudModel is currently active) and forwards
    /// external Show/Hide requests to the View. Does NOT decide how Show/Hide looks (View's job)
    /// and does NOT decide whether a skill switch is legal (Model's job) - it only relays input
    /// and keeps the subscription lifecycle correct.
    /// </summary>
    public class HUDPresenter : IDisposable
    {
        private readonly IHUDView _view;
        private readonly PlayerStats _model;
        private readonly PlayerSkillController _abilityController;

        public HUDPresenter(IHUDView view, PlayerStats  model,PlayerSkillController skillController)
        {
            _view = view;
            _model = model;
            _abilityController = skillController;
        }

        /// <summary>
        /// Call once whenever the active HudModel changes (e.g. from a PlayerModelProvider callback).
        /// </summary>
        public void Initialize()
        {
            Unbind();

            
            _model.OnHealthChanged += RefreshHealth;
            _model.OnManaChanged += RefreshMana;
            _abilityController.OnActiveAbilityChanged  += RefreshActiveAbility;
            _abilityController.OnAbilitySlotChanged  += RefreshAbilitySlot;

            RefreshHealth(_model.CurrentHealth, _model.MaxHealth);
            RefreshMana(_model.CurrentMana, _model.MaxMana);
            RefreshAllAbility();
            RefreshActiveAbility(_abilityController.ActiveAbility);
        }

        public void Unbind()
        {
            if (_model == null)
                return;

            _model.OnHealthChanged -= RefreshHealth;
            _model.OnManaChanged -= RefreshMana;
        }
        
        public void Dispose()
        {
            Unbind();
        }

        // Presenter decides WHEN Show/Hide fires (driven by Suppression/Stack signals, see
        // HudVisibilityController from earlier); View decides HOW it looks.
        public void Show(bool instant = false) => _view.Show(instant);
        public void Hide(bool instant = false) => _view.Hide(instant);
        
        private void RefreshHealth(float currentHealth, float maxHealth)
        {
            if (_model == null) return;
            _view.SetHealth(currentHealth, maxHealth);
        }

        private void RefreshMana(float currentMana, float maxMana)
        {
            if (_model == null) return;
            _view.SetMana(currentMana, maxMana);
        }

        // ABILITY SECTION
        private void RefreshActiveAbility(AbilityDisplayDataSO  skill)
        {
            _view.SetActiveAbility(skill);
        }

        private void RefreshAbilitySlot(
            int slotIndex,
            AbilityDisplayDataSO  skill)
        {
            _view.SetAbility(slotIndex, skill);
        }

        private void RefreshAllAbility()
        {
            for (int i = 0; i < _abilityController.AbilitySlotCount; i++)
            {
                _view.SetAbility(i, _abilityController.GetSkill(i));
            }
        }
    }
}