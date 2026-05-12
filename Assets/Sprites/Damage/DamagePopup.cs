using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro _tmp;
    private float _lifetime = 0.8f;
    private float _timer;
    private Vector3 _moveDir;

    public void Setup(int damage, bool isCrit = false)
    {
        _tmp = GetComponent<TextMeshPro>();
        _tmp.text = damage.ToString();
        _tmp.color = isCrit ? Color.yellow : Color.white;
        _tmp.fontSize = isCrit ? 12 : 8;
        _timer = _lifetime;
        _moveDir = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0);
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        transform.position += _moveDir * Time.deltaTime;
        _moveDir -= _moveDir * 8f * Time.deltaTime;

        if (_timer < _lifetime * 0.4f)
        {
            Color c = _tmp.color;
            c.a = _timer / (_lifetime * 0.4f);
            _tmp.color = c;
        }

        if (_timer <= 0) Destroy(gameObject);
    }
}