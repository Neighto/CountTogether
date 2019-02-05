using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimStateInfo {
	public string name;
	public GameObject prefab;
	public int frameRate = 12;
}

public class AnimState : MonoBehaviour {
	struct Anim {
		public GameObject obj;
		public QuillAnimation qanim;
	}

	[SerializeField]
	public AnimStateInfo[] animations;

	private Dictionary<string, Anim> _instantiatedAnims = new Dictionary<string, Anim>();
	private string current;
	private string loopingAnim;

	// Use this for initialization
	void Start () {
		if(animations.Length == 0) {
			Debug.Log("Unspecified Animation");
		}

		// get position, rotation from placeholder
		var visuals = this.transform.Find("visuals") as Transform;
		var pos = visuals.position;
		var rot = visuals.rotation;

		Destroy(visuals.gameObject);

		// instantiate all animations
		foreach(var anim in animations) {
			var obj = Instantiate(anim.prefab, pos, rot, this.transform);
			obj.SetActive(false);

			var qanim = obj.GetComponent<QuillAnimation>();
			if(qanim == null) {
				qanim = obj.AddComponent<QuillAnimation>();
				qanim.frameRate = anim.frameRate;
			}

			_instantiatedAnims.Add(anim.name, new Anim() {obj = obj, qanim = qanim});
		}

		current = animations[0].name;
		SwitchAnim(current);
	}

	public void SwitchAnim(string name, bool reset = true) {
		_instantiatedAnims[current].obj.SetActive(false);

		current = name;
		_instantiatedAnims[current].obj.SetActive(true);
		_instantiatedAnims[current].qanim.Reset();
	}

	public void EnsureState(string name) {
		if(current != name) {
			SwitchAnim(name);
		}
	}

	public void PlayOnce(string name) {
		if(loopingAnim != null)
			ReturnToLoop();

		_instantiatedAnims[current].obj.SetActive(false);
		
		loopingAnim = current;
		current = name;

		_instantiatedAnims[current].obj.SetActive(true);
		_instantiatedAnims[current].qanim.AddOnEndHandler(ReturnToLoop);
		_instantiatedAnims[current].qanim.Reset();
	}

	void ReturnToLoop() {
		_instantiatedAnims[current].qanim.RemoveOnEndHandler(ReturnToLoop);
		_instantiatedAnims[current].obj.SetActive(false);

		SwitchAnim(loopingAnim, false);
		loopingAnim = null;
	}
}
