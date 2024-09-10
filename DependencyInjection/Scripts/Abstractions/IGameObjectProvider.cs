using UnityEngine;

namespace DependencyInjection.Abstractions
{
    public interface IGameObjectProvider
    {
        GameObject CreateGameObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent);
    }
}