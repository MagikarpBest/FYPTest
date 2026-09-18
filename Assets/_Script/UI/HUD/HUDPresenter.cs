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
        // ===== Model =====
        // For binding entities invoked actions
        // Health bar
        // Mana bar
        // Skills list (using skill interaction handle in model itself, switch skills from view or maybe model itself still)
        private HUDModel _model;
        // =================

        public HUDPresenter(IHUDView view)
        {
            _view = view;
        }

        /// <summary>
        /// Call once whenever the active HudModel changes (e.g. from a PlayerModelProvider callback).
        /// </summary>
        public void Bind(HUDModel model)
        {
            Unbind();
            _model = model;

            _model.OnHealthChanged += RefreshHealth;
            _model.OnManaChanged += RefreshMana;

            RefreshHealth(_model.CurrentHealth, _model.MaxHealth);
            RefreshMana(_model.CurrentMana, _model.MaxMana);
        }

        public void Unbind()
        {
            if (_model == null)
                return;

            _model.OnHealthChanged -= RefreshHealth;
            _model.OnManaChanged -= RefreshMana;
            _model = null;
        }
        
        public void Dispose()
        {
            Unbind();
        }

        // Presenter decides WHEN Show/Hide fires (driven by Suppression/Stack signals, see
        // HudVisibilityController from earlier); View decides HOW it looks.
        public void Show(bool instant = false) => _view.Show(instant);
        public void Hide(bool instant = false) => _view.Hide(instant);

        private void RefreshHealth(int currentHealth, int maxHealth)
        {
            if (_model == null) return;
            _view.SetHealth(currentHealth, maxHealth);
        }

        private void RefreshMana(float currentMana, float maxMana)
        {
            if (_model == null) return;
            _view.SetMana(currentMana, maxMana);
        }
    }
}