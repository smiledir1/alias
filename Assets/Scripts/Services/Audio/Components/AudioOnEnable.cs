namespace Services.Audio.Components
{
    public class AudioOnEnable : AudioComponent
    {
        private void OnEnable()
        {
            PlaySound();
        }
    }
}