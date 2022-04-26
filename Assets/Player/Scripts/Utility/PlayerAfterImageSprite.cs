using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _playerSpriteRenderer;

    private Color _color;

    private float _activeTime = 0.1f;
    private float timeAcrivated;
    private float _alpha;
    private float _alphaSet = 0.8f;
    [SerializeField]
    private float _alphaMultiplayer = 0.8f;

    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerSpriteRenderer = _player.GetComponent<SpriteRenderer>();

        _alpha = _alphaSet;

        _spriteRenderer.sprite = _playerSpriteRenderer.sprite;

        if (_player.localScale.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
        


        transform.position = _player.position;
        transform.rotation = _player.rotation;
        timeAcrivated = Time.time;
    }

    private void Update()
    {
        _alpha *= _alphaMultiplayer;
        _color = new Color(1f, 1f, 1f, _alpha*_alphaMultiplayer);

        if (Time.time >= (timeAcrivated +_activeTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
