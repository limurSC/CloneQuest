using UnityEngine;
using DG.Tweening;

public class RigidbodySoftReset : MonoSoftResetListener
{
    public (Vector2 position, float rotation) Initial { get => (_initialPosition, _initialRotation); set => (_initialPosition, _initialRotation) = value; }

    [SerializeField] private Rigidbody2D _rigidbody;
    private Vector2 _initialPosition;
    private float _initialRotation;

    public void SoftResetHandler(float duration)
    {
        _rigidbody.bodyType = RigidbodyType2D.Static;
        DOTween.Sequence().SetLink(gameObject).SetEase(Ease.InOutCubic)
            .Append(transform.DOMove(_initialPosition, duration))
            .Join(transform.DORotate(new(0, 0, _initialRotation), duration))
            .AppendCallback(() => _rigidbody.bodyType = RigidbodyType2D.Dynamic);
    }

    private new void Awake()
    {
        (_initialPosition, _initialRotation) = (_rigidbody.position, _rigidbody.rotation);
        StartActions.AddListener(SoftResetHandler);
        base.Awake();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_rigidbody == null) { _rigidbody = GetComponent<Rigidbody2D>(); }
    }
#endif
}