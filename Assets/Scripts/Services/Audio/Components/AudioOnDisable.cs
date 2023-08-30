namespace Services.Audio.Components
{
    public class AudioOnDisable : AudioComponent
    {
        private void OnDisable()
        {
            PlaySound();
        }
    }
}