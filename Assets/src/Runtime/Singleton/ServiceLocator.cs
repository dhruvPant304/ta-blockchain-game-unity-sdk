using System;
using System.Collections.Generic;
using UnityEngine;

namespace TA.Services{
public class ServiceLocator : Singleton<ServiceLocator>{
	Dictionary<Type,MonoBehaviour> _services = new();

	protected override void Initialize(){
		Debug.Log("Service locator intialised");
	}

	public void RegisterService<T>(Service<T> service, bool persistant = false) where T: Service<T>{
		if(_services.ContainsKey(service.serviceType)){
			if(_services[service.serviceType] != null)
			throw new Exception($"Service type '{service.serviceType.ToString()}' is already registerd");
		}
		_services[service.serviceType] = service;
		if(persistant) MakeGameObjectPersistant(service.gameObject);	
		Debug.Log($"Service:{service.serviceType} registered");
	}

	public T GetService<T>() where T:Service<T>{
		if(!_services.ContainsKey(typeof(T))){
			throw new Exception($"Service of type '{typeof(T)}' not registered");
		}
		return _services[typeof(T)].GetComponent<T>();
	}

	private void MakeGameObjectPersistant(GameObject gameObject){
		gameObject.transform.SetParent(null);
		DontDestroyOnLoad(gameObject);
	}

	public void CloseServices(){
		foreach(var service in _services){
			Destroy(service.Value.gameObject);
		}

		_services = new();
		Destroy(gameObject);
	}
}
}

