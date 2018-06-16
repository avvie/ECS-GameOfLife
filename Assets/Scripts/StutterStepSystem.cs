using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class StutterStepSystem : ComponentSystem {
    [Inject] UpdateStateSystem _USS;

    protected override void OnUpdate() {
        if (Input.GetKeyUp(KeyCode.Space))
            _USS.Enabled = !_USS.Enabled;
    }


}
