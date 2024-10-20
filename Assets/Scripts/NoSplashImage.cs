using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

[Preserve]  //防止打包的时候没有被打包进程序
public class NoSplashImage : MonoBehaviour
{
    //在启动画面显示之前执行这个方法
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Run()
    {
        Task.Run(() =>
        {
            SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
        });
    }
}