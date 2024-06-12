using System;
using System.Collections.Generic;
using UnityEngine;

namespace TA{
public class ServiceLocator : Singleton<ServiceLocator>{
	Dictionary<Type,MonoBehaviour> _services = new();

	protected override void Initialize(){
		Debug.Log("Service locator intialised");
	}

	public void RegisterService<T>(Service<T> service, bool asSingle = false) where T: Service<T>{
		if(_services.ContainsKey(service.serviceType)){
			throw new Exception($"Service type '{service.serviceType.ToString()}' is already registerd");
		}

		_services[service.serviceType] = service;
		if(asSingle) DontDestroyOnLoad(service.gameObject);
		Debug.Log($"Service:{service.serviceType} registered");
	}

	public T GetService<T>() where T:Service<T>{
		if(!_services.ContainsKey(typeof(T))){
			throw new Exception($"Service of type '{typeof(T)}' not registered");
		}
		return _services[typeof(T)].GetComponent<T>();
	}

}
}

