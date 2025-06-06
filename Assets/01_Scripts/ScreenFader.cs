using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WallDefense
{
    public class ScreenFader : MonoBehaviour
    {
        public float FadeDuration = 1f;
        private int _fadeAmount = Shader.PropertyToID("_FadeAmount");
        private int _useShutters = Shader.PropertyToID("_UseShuttersw");

        private Image _image;
        private Material _material;

        private void Awake()
        {
            _image = GetComponent<Image>();
            Material mat = _image.material;
            _image.material = new Material(mat);
            _material = _image.material;
        }
        public void StartFade()
        {
            StartCoroutine(StartFadeCoroutine());
        }
        public void EndFade()
        {
            StartCoroutine(EndFadeCoroutine());
        }

        public IEnumerator StartFadeCoroutine()
        {
            _material.SetFloat(_fadeAmount, 0f);
            yield return HandleFade(1f, 0f);
        }
        public IEnumerator EndFadeCoroutine()
        {
            _material.SetFloat(_fadeAmount, 1f);
            yield return HandleFade(0f, 1f);
        }


        private IEnumerator HandleFade(float targetAmount, float startAmount)
        {
            float elapsedTime = 0f;
            while (elapsedTime < FadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float lerpedAmount = Mathf.Lerp(startAmount, targetAmount, elapsedTime / FadeDuration);
                _material.SetFloat(_fadeAmount, lerpedAmount);

                yield return null;
            }

            _material.SetFloat(_fadeAmount, targetAmount);

        }
    }
}
