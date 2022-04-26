using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _firePoint;

    [SerializeField] private float _fireSpeed;

    private SpriteRenderer _projectileSprite;
    private bool _thisDirection;

   
    public void SetDirection(bool _currentFacing)
    {
        _thisDirection = _currentFacing;
    }

    private void Awake()
    {
       

    }
    private void ProjectileLaunch()
    {
        GameObject _currentProjectile = Instantiate<GameObject>(_projectile, _firePoint.position, Quaternion.identity);

        Rigidbody2D _currentProjectileVelocity = _currentProjectile.GetComponent<Rigidbody2D>();

        _projectileSprite = _currentProjectile.GetComponent<SpriteRenderer>();

        if (_thisDirection)
        {
            if (_projectileSprite.flipX)
            {
                _projectileSprite.flipX = !_projectileSprite.flipX;

            }
           
              
            _currentProjectileVelocity.velocity = new Vector2(_fireSpeed * 1, _currentProjectileVelocity.velocity.y);
       
        }
        else
        {
            if (!_projectileSprite.flipX)
            {
                _projectileSprite.flipX = !_projectileSprite.flipX;
               
            }
          
               


            _currentProjectileVelocity.velocity = new Vector2(_fireSpeed * -1, _currentProjectileVelocity.velocity.y);

        }
          
    }
    
}
