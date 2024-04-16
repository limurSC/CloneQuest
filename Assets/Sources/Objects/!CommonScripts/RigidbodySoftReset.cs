using UnityEngine;
using DG.Tweening;

public class RigidbodySoftReset : MonoBehaviour, ISoftReset
{
    [SerializeField] private Rigidbody2D _rigidbody;
    private Vector2 _initialPosition;
    private float _initialRotation;

    public void SoftReset(float duration)
    {
        _rigidbody.bodyType = RigidbodyType2D.Static;
        DOTween.Sequence().SetLink(gameObject).SetEase(Ease.InOutCubic)
            .Append(transform.DOMove(_initialPosition, duration))
            .Join(transform.DORotate(new(0, 0, _initialRotation), duration))
            .AppendCallback(() => _rigidbody.bodyType = RigidbodyType2D.Dynamic);
    }

    private void Awake()
    {
        _initialPosition = _rigidbody.position;
        _initialRotation = _rigidbody.rotation;
        EventBus.Subscribe<ISoftReset>(this);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<ISoftReset>(this);
    }

    private void OnValidate()
    {
        if (_rigidbody == null) { _rigidbody = GetComponent<Rigidbody2D>(); }
    }
}