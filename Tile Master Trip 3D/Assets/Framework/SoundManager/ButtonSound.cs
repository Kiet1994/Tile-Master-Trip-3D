using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] private string soundId;

    #endregion Inspector Variables

    #region Unity Methods

    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(PlaySound);
    }

    #endregion Unity Methods

    #region Private Methods

    private void PlaySound()
    {
        if (SoundManager.Exists())
        {
            SoundManager.Instance.Play(soundId);
        }
    }

    #endregion Private Methods
}