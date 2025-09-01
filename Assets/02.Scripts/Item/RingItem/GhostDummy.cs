using System.Collections.Generic;
using UnityEngine;

public class GhostDummy : MonoBehaviour
{
    [SerializeField] List<DummyFeature> _dummyFeatuers = new List<DummyFeature>();

    [SerializeField] MeshRenderer _faceMeshRenderer;
    [SerializeField] List<Material> _faceMaterials = new List<Material>(); // 0: 디폴트 머티리얼, 1: 주근깨 머티리얼


    // 현재 라운드 정답에 따라, 더미를 정답 귀신과 동일한 모습으로 세팅하는 함수
    public void SetDummy()
    {
        Answer answer = RoundManager.Instance.GetCurrentAnswer();
        StudentSO studentSO = answer.studentSO;

        foreach(var feature in studentSO.features)
        {
            foreach(DummyFeature dummy in _dummyFeatuers)
            {
                if (dummy.featureSO.id == feature.id)
                {
                    dummy.gameObject.SetActive(true);
                    break;
                }
                
                dummy.gameObject.SetActive(false);
            }

            if(feature.id == "Feature_014")
            {
                _faceMeshRenderer.material = _faceMaterials[1];
            }
            else
            {
                _faceMeshRenderer.material = _faceMaterials[0];
            }
        }
    }
}
