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
        private readonly IPlayerStatus _model;

        public HUDPresenter(IHUDView view, IPlayerStatus model)
        {
            _view = view;
            _model = model;
        }

        /// <summary>
        /// Call once whenever the active HudModel changes (e.g. from a PlayerModelProvider callback).
        /// </summary>
        public void Initialize()
        {
            Unbind();

            _view.OnSkillSwitchInput += HandleSkillSwitchInput;
            
            _model.OnHealthChanged += RefreshHealth;
            _model.OnManaChanged += RefreshMana;
            _model.OnActiveSkillChanged += RefreshActiveSkill;
            _model.OnSkillSlotChanged += RefreshSkillSlot;

            RefreshHealth(_model.CurrentHealth, _model.MaxHealth);
            RefreshMana(_model.CurrentMana, _model.MaxMana);
            RefreshAllSkills();
            RefreshActiveSkill(_model.ActiveSkill);
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

        private void HandleSkillSwitchInput(int direction)
        {
            bool switched = _model.TrySwitchSkill(direction);

            if (switched)
            {
                _view.PlaySkillSwitchPressed();
            }
            else
            {
                _view.PlaySkillSwitchFailed();
            }
        }
        
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

        private void RefreshActiveSkill(PlayerSkill skill)
        {
            _view.SetActiveSkill(skill);
        }

        private void RefreshSkillSlot(
            int slotIndex,
            PlayerSkill skill)
        {
            _view.SetSkill(slotIndex, skill);
        }

        private void RefreshAllSkills()
        {
            for (int i = 0; i < _model.SkillSlotCount; i++)
            {
                _view.SetSkill(i, _model.GetSkill(i));
            }
        }
    }
}