using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TESTLevelMenu : MonoBehaviour
{
    [SerializeField] RectTransform _container;
    [SerializeField] Button _buttonTemplate;
    [HideInInspector][SerializeField] string[] _levelIds;

    private Button[] _buttons;

    private void CreateButtons()
    {
        _buttons = _levelIds.Select(id => Instantiate(_buttonTemplate, _container)).ToArray();
        for (var i = 0; i < _buttons.Length; i++)
        {
            var button = _buttons[i];
            var levelIndex = i;
            button.GetComponentInChildren<TMP_Text>().text = $"{levelIndex + 1}";
            button.onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }

    private void LoadLevel(int index)
    {
        var context = new LevelContext(index, Array.AsReadOnly(_levelIds), SceneManager.GetActiveScene().name);
        LevelManager.Load(context);
    }

    private void Awake() => CreateButtons();

#if UNITY_EDITOR
    [SerializeField] SceneAsset[] _levelAssets;
    private void OnValidate()
    {
        _levelIds = _levelAssets
            .Select(scene => scene.name)
            .ToArray();
    }
#endif
}
