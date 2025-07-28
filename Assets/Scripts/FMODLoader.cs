using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO disable Load Banks in FMOD settings, I do not see any settings without the project initted
public class FMODLoader : MonoBehaviour
{
   [FMODUnity.BankRef]
   public List<string> Banks = new List<string>();
   
   // The name of the scene to load and switch to
   public string Scene = null;
   
   private void Awake()
   {
      StartCoroutine(LoadGameAsync());
   }

   IEnumerator LoadGameAsync()
   {
      // Start an asynchronous operation to load the scene
      AsyncOperation async = SceneManager.LoadSceneAsync(Scene);

      // Don't let the scene start until all Studio Banks have finished loading
      async.allowSceneActivation = false;
      
      // Iterate all the Studio Banks and start them loading in the background
      // including the audio sample data
      foreach (var bank in Banks)
      {
         FMODUnity.RuntimeManager.LoadBank(bank, true);
      }

      // Keep yielding the co-routine until all the bank loading is done
      // (for platforms with asynchronous bank loading)
      while (!FMODUnity.RuntimeManager.HaveAllBanksLoaded)
      {
         yield return null;
      }

      // Keep yielding the co-routine until all the sample data loading is done
      while (FMODUnity.RuntimeManager.AnySampleDataLoading())
      {
         yield return null;
      }

      // Allow the scene to be activated. This means that any OnActivated() or Start()
      // methods will be guaranteed that all FMOD Studio loading will be completed and
      // there will be no delay in starting events
      async.allowSceneActivation = true;

      // Keep yielding the co-routine until scene loading and activation is done.
      while (!async.isDone)
      {
         yield return null;
      }
   }
}
