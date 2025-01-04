using System;
using UnityEngine;

namespace TA.Services{
public abstract class Service<T> : MonoBehaviour where T : Service<T>{
	public bool registerAsSingle;
	public Type serviceType => typeof(T);	
	public void Awake(){
		ServiceLocator.Instance.RegisterService(this, registerAsSingle);
		OnInitialize();
	}
	public static T Get() => ServiceLocator.Instance.GetService<T>();
	protected virtual void OnInitialize(){}
}}
