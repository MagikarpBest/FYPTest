namespace Player
{
    public interface IPlayerSkillHandler
    {
        // Player prepare to execute skill
        void Begin(PlayerSkillContext context);

        // Player left-click to execute skill
        SkillConfirmResult Confirm();

        // Player right-click to cancel skill
        void Cancel();

        // Visual layer actively update the preview screren etc the pillar creation.
        // Might not needed.
        void Update();
    }
}