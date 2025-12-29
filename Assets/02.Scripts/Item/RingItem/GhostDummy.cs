using System.Collections.Generic;
using UnityEngine;

public class GhostDummy : MonoBehaviour
{
    [SerializeField] List<DummyFeature> _dummyFeatuers = new List<DummyFeature>();

    [SerializeField] MeshRenderer _faceMeshRenderer;
    [SerializeField] List<Material> _faceMaterials = new List<Material>(); // 0: 디폴트 머티리얼, 1: 주근깨 머티리얼


    // 현재 라운드 정답에 따라, 더미를 정답 귀신과 동일한 모습으로 세팅하는 함수
    public void SetDummy(Answer answer)
    {
        var studentSO = answer.studentSO;

        // 1) 학생이 가진 feature id 집합
        var owned = new HashSet<string>();
        foreach (var f in studentSO.features)
            owned.Add(f.id);

        // 2) 주근깨 여부
        bool hasFreckles = owned.Contains("Feature_014");

        // 3) 더미 전체 활성/비활성 (주근깨는 더미가 아니라 얼굴 머티리얼로 처리)
        foreach (var dummy in _dummyFeatuers)
        {
            if (dummy == null || dummy.featureSO == null) continue;

            bool active =
                dummy.featureSO.id != "Feature_014" && // 주근깨는 여기서 처리 X
                owned.Contains(dummy.featureSO.id);

            dummy.gameObject.SetActive(active);
        }

        // 4) 얼굴 머티리얼은 마지막에 한 번만
        _faceMeshRenderer.material = _faceMaterials[hasFreckles ? 1 : 0];

        Debug.Log("더미 설정 완료");
    }
}
