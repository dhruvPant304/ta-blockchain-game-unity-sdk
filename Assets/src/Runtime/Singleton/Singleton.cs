using UnityEngine;

namespace TA{
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>{
	static T _instance;
	public static T Instance { get {
		if(_instance == null){
			_instance = GameObject.FindObjectOfType<T>();	
		}			
		return _instance;	
	}}

	public void Awake(){
		if(Instance != this){
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		Initialize();	
	}

	void NameGameObject(){
		transform.name = typeof(T).ToString() + " (Singleton)";
	}

	protected abstract void Initialize();
}
}

