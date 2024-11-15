using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace CHARACTERS
{
    public class Character_Model3D : Character
    {
        private const string CHARACTER_RENDER_GROUP_PREFAB_NAME_FORMAT = "RenderGroup - [{0}]";
        private const string CHARACTER_RENDER_TEXTURE_NAME_FORMAT = "RenderTexture";
        private const int CHARACTER_STACKING_DEPTH = 15;
        private const float EXPRESSION_TRANSITION_SPEED = 100f;
        private const float DEFAULT_TRANSITION_SPEED = 3f;
        private const float DEFAULT_FACING_DIRECTION_VALUE = 25f;
        private GameObject renderGroup;
        private Camera camera;
        private Transform modelContainer, model;
        private Animator modelAnimator;
        private SkinnedMeshRenderer modelExpressionController;
        private RawImage renderer;
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();
        private CanvasGroup rendererCG => renderer.GetComponent<CanvasGroup>();
        private Dictionary<string, Coroutine> expressionCoroutines = new Dictionary<string, Coroutine>();
        public override bool isVisible { get => isRevealing || rootCG.alpha > 0; set => rootCG.alpha = value ? 1 : 0; }
        private Coroutine co_fadingOutOldRenderers = null;
        private bool isFadingOutOldRenderers => co_fadingOutOldRenderers != null;
        private float oldRendererFadeOutSpeedMultiplier = DEFAULT_TRANSITION_SPEED;
        private struct OldRenderer
        {
            public CanvasGroup oldCG;
            public RawImage oldImage;
            public GameObject oldRenderGroup;
            public OldRenderer(CanvasGroup oldCG, RawImage oldImage,GameObject oldRenderGroup)
            {
                this.oldCG = oldCG;
                this.oldImage = oldImage;
                this.oldRenderGroup = oldRenderGroup;
            }
        }
        private List<OldRenderer> oldRenderers = new List<OldRenderer>();
        public Character_Model3D(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            Debug.Log($"Create Model3D Character: '{name}'");
            GameObject renderGroupPrefab = Resources.Load<GameObject>(rootAssetsFolder + '/' + string.Format(CHARACTER_RENDER_GROUP_PREFAB_NAME_FORMAT, config.name));
            renderGroup = Object.Instantiate(renderGroupPrefab, characterManager.characterPanel_Model3D);
            renderGroup.name = string.Format(CHARACTER_RENDER_GROUP_PREFAB_NAME_FORMAT, name);
            renderGroup.SetActive(true);
            camera = renderGroup.GetComponentInChildren<Camera>();
            modelContainer = camera.transform.GetChild(0);
            model = modelContainer.GetChild(0);
            modelAnimator = model.GetComponent<Animator>();
            modelExpressionController = model.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(sm => sm.sharedMesh.blendShapeCount > 0);
            renderer = animator.GetComponentInChildren<RawImage>();
            RenderTexture renderTex = Resources.Load<RenderTexture>(rootAssetsFolder + '/' + CHARACTER_RENDER_TEXTURE_NAME_FORMAT);
            RenderTexture newTex = new RenderTexture(renderTex);
            renderer.texture = newTex;
            camera.targetTexture = newTex;
            int modelsInScene = characterManager.GetCharacterCountFromCharacterType(CharacterType.Model3D);
            renderGroup.transform.position += Vector3.down * (CHARACTER_STACKING_DEPTH * modelsInScene);
        }
        public void SetMotion(string motionName)
        {
            modelAnimator.Play(motionName);
        }
        public void SetExpression(string blendShapeName, float weight, float speedMultiplier = 1, bool immediate = false)
        {
            if(modelExpressionController == null)
            {
                Debug.LogWarning($"Character {name} does not have an expression controller. Blend Shpes may be null. [{modelExpressionController.name}]");
                return;
            }
            if(expressionCoroutines.ContainsKey(blendShapeName))
            {
                characterManager.StopCoroutine(expressionCoroutines[blendShapeName]);
                expressionCoroutines.Remove(blendShapeName);
            }
            Coroutine expressionCoroutine = characterManager.StartCoroutine(ExpressionCoroutine(blendShapeName, weight, speedMultiplier, immediate));
            expressionCoroutines[blendShapeName] = expressionCoroutine;
        }
        private IEnumerator ExpressionCoroutine(string blendShapeName, float weight, float speedMultiplier = 1, bool immediate = false)
        {
            int blendShapeIndex=modelExpressionController.sharedMesh.GetBlendShapeIndex(blendShapeName);
            if(blendShapeIndex==-1)
            {
                Debug.LogWarning($"Character {name} does not have a blend shape by the name of '{blendShapeName}' [{modelExpressionController.name}]");
                yield break;
            }
            if(immediate)
            {
                modelExpressionController.SetBlendShapeWeight(blendShapeIndex, weight);
            }
            else
            {
                float currentValue = modelExpressionController.GetBlendShapeWeight(blendShapeIndex);
                while(currentValue!=weight)
                {
                    currentValue=Mathf.MoveTowards(currentValue, weight, Time.deltaTime * EXPRESSION_TRANSITION_SPEED * speedMultiplier);
                    modelExpressionController.SetBlendShapeWeight(blendShapeIndex, currentValue);
                    yield return null;
                }
            }
            expressionCoroutines.Remove(blendShapeName);
        }
        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0;
            CanvasGroup self = rootCG;
            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime);
                yield return null;
            }
            co_revealing = null;
            co_hiding = null;
        }
        public override void SetColor(Color color)
        {
            base.SetColor(color);
            renderer.color = color;
            foreach(var or in oldRenderers)
            {
                or.oldImage.color = color;
            }
        }
        public override IEnumerator ChangingColor(float speedMultiplier)
        {
            yield return ChangingRendererColor(speedMultiplier);
            co_changingColor = null;
        }
        public override IEnumerator Highlighting(float speedMultiplier)
        {
            if(!isChangingColor)
                yield return ChangingRendererColor(speedMultiplier);
            co_highlighting = null;
        }
        public override IEnumerator FaceDirection(bool faceleft, float speedMultiplier, bool immediate)
        {
            Vector3 facingAngle = new Vector3(0, (facingLeft ? DEFAULT_FACING_DIRECTION_VALUE : -DEFAULT_FACING_DIRECTION_VALUE), 0);
            if(immediate)
            {
                modelContainer.localEulerAngles = facingAngle;
            }
            else
            {
                CreatNewCharacterRenderingInstance();
                modelContainer.localEulerAngles = facingAngle;
                oldRendererFadeOutSpeedMultiplier = speedMultiplier;
                if (!isFadingOutOldRenderers)
                    co_fadingOutOldRenderers = characterManager.StartCoroutine(FadingOutOldRenderers());
                CanvasGroup newRenderer = rendererCG;
                while(newRenderer.alpha!=1)
                {
                    float speed = DEFAULT_TRANSITION_SPEED * Time.deltaTime * speedMultiplier;
                    newRenderer.alpha = Mathf.MoveTowards(newRenderer.alpha,1,speed);
                    yield return null;
                }
            }
            co_flipping = null;
        }
        public override void OnReceiveCastingExpression(int layer, string expression)
        {
            SetExpression(expression, 1);
        }
        private IEnumerator ChangingRendererColor(float speedMultiplier)
        {
            Color oldColor = renderer.color;
            float colorPercent = 0;
            while(colorPercent!=1)
            {
                colorPercent += DEFAULT_TRANSITION_SPEED * speedMultiplier * Time.deltaTime;
                renderer.color = Color.Lerp(oldColor,displayColor, colorPercent);
                foreach (var or in oldRenderers)
                    or.oldImage.color = renderer.color;
                yield return null;
            }
            co_changingColor = null;
        }
        private void CreatNewCharacterRenderingInstance()
        {
            oldRenderers.Add(new OldRenderer(rendererCG, renderer, renderGroup));
            //GameObject renderGroupPrefab = Resources.Load<GameObject>(rootAssetsFolder + '/' + string.Format(CHARACTER_RENDER_GROUP_PREFAB_NAME_FORMAT, config.name));
            renderGroup = Object.Instantiate(renderGroup, renderGroup.transform.parent);
            renderGroup.name = string.Format(CHARACTER_RENDER_GROUP_PREFAB_NAME_FORMAT, name);
            camera = renderGroup.GetComponentInChildren<Camera>();
            modelContainer = camera.transform.GetChild(0);
            model = modelContainer.GetChild(0);
            modelAnimator = model.GetComponent<Animator>();
            modelExpressionController = model.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(sm => sm.sharedMesh.blendShapeCount > 0);
            string rendererName = renderer.name;
            Texture oldRenderTexture = renderer.texture;
            renderer = Object.Instantiate(renderer.gameObject, renderer.transform.parent).GetComponent<RawImage>();
            renderer.name = rendererName;
            rendererCG.alpha = 0;
            RenderTexture newTex = new RenderTexture(oldRenderTexture as RenderTexture);
            renderer.texture = newTex;
            camera.targetTexture = newTex;
            //int modelsInScene = characterManager.GetCharacterCountFromCharacterType(CharacterType.Model3D);
            for (int i = 0; i < oldRenderers.Count; i++)
                oldRenderers[i].oldRenderGroup.transform.localPosition = Vector3.zero + (Vector3.right * i);
            renderGroup.transform.position = Vector3.zero + (Vector3.right * (CHARACTER_STACKING_DEPTH * oldRenderers.Count));
        }
        private IEnumerator FadingOutOldRenderers()
        {
            while(oldRenderers.Any(o=>o.oldCG.alpha>0))
            {
                float speed = DEFAULT_TRANSITION_SPEED * Time.deltaTime * oldRendererFadeOutSpeedMultiplier;
                foreach(var or in oldRenderers)
                {
                    or.oldCG.alpha = Mathf.MoveTowards(or.oldCG.alpha,0,speed);
                }
                yield return null;
            }
            foreach(var or in oldRenderers)
            {
                Object.Destroy(or.oldRenderGroup);
                Object.Destroy(or.oldCG.gameObject);
            }
            oldRenderers.Clear();
            co_fadingOutOldRenderers = null;
        }
    }
}