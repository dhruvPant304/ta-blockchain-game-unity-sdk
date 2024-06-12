using System;
using TA;
using UnityEngine;

public abstract class Service<T> : MonoBehaviour where T : Service<T>{
	public bool registerAsSingle;
	public Type serviceType => typeof(T);	
	public void Awake(){
		ServiceLocator.Instance.RegisterService(this, registerAsSingle);
		OnInitialize();
	}
	protected abstract void OnInitialize();
}
