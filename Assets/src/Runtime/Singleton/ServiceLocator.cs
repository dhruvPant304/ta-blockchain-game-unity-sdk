using System;
using System.Collections.Generic;
using UnityEngine;

namespace TA.Services{
public class ServiceLocator : Singleton<ServiceLocator>{
	Dictionary<Type,MonoBehaviour> _services = new();
	Dictionary<Type, Action> _registrationCallbacks = new();

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

		//Invoking registration callabacks
		if(_registrationCallbacks.ContainsKey(typeof(T))){
			_registrationCallbacks[typeof(T)]?.Invoke();
		}
		Debug.Log($"Service:{service.serviceType} registered");
	}

	public void OnServiceRegistered<T>(Action onRegister) where T : Service<T> {		
		if(!_registrationCallbacks.ContainsKey(typeof(T))){
			_registrationCallbacks.Add(typeof(T), onRegister);
		}else{
			_registrationCallbacks[typeof(T)] += onRegister;
		}

		//Invoking the callback if the serivce is already registered
		if(_services.ContainsKey(typeof(T))){
			onRegister?.Invoke();
		}
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

