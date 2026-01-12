using UnityEngine;

namespace LuneRun
{
    public class SetupInstructions : MonoBehaviour
    {
        [TextArea(10, 30)]
        [SerializeField] private string instructions = @"LUNE RUN - Unity Port Setup Instructions

This is a port of the Flash game 'Lunerun' to Unity.

1. BASIC SETUP:
   - Attach the 'Bootstrapper' script to any GameObject in your starting scene.
   - Ensure you have a Camera tagged 'MainCamera' and a Canvas for UI.

2. PREFAB CREATION (optional):
   - Create an empty GameObject named 'GameManagers'.
   - Add the following components: GameManager, AudioManager, HighscoreManager.
   - Save as a prefab for easy reuse.

3. UI SETUP:
   - Create a Canvas with Screen Space Overlay.
   - Add buttons for sound, music, help, fullscreen, tweet, link, survivors, paypal.
   - Hook up button events to MenuManager methods.

4. AUDIO SETUP:
   - Assign sound and music clips in the AudioManager inspector.
   - Configure volumes as desired.

5. LEVEL DESIGN:
   - Use TrackGenerator to create tracks for each level.
   - Adjust segment length, slope angles, and visual appearance.

6. PLAYER CONTROLS:
   - The PlayerController uses CharacterController for movement.
   - Hold SPACE to run, release to jump.
   - Press SPACE in air to land quicker.

7. HIGHSCORES:
   - Configure HighscoreManager with UI elements.
   - Implement IRunnerApi for online functionality or use LocalRunnerApi for testing.

8. START THE GAME:
   - The Bootstrapper will automatically spawn managers and go to the main menu.

NOTES:
- This is a basic port focusing on core gameplay.
- Many features from the original are stubbed and need implementation.
- Customize visuals, sounds, and gameplay to match the original.

For questions, refer to the original ActionScript code in the Flash反编译 folder.";

        private void Start()
        {
            Debug.Log("Lune Run Unity Port - Setup Instructions");
            Debug.Log(instructions);
        }
    }
}