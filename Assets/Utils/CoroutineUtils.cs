using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtils {

	public static IEnumerator DisableAnimatorBooleanProperty(Animator animator, string propertyName)
    {
        yield return new WaitForEndOfFrame();
        animator.SetBool(propertyName, false);
    }
}
