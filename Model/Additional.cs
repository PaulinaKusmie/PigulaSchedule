//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//#if ANDROID
//using Android.Gms.Tasks;  
//#endif

//namespace PigulaSchedule.Model
//{
//    #if ANDROID
//    public class SuccessListener<T> : Java.Lang.Object, IOnSuccessListener
//        where T : Java.Lang.Object
//    {
//        private readonly Action<T> _action;
//        public SuccessListener(Action<T> action) => _action = action;
//        public void OnSuccess(Java.Lang.Object result) => _action((T)result);
//    }

//    public class FailureListener : Java.Lang.Object, IOnFailureListener
//    {
//        private readonly Action<Java.Lang.Exception> _action;
//        public FailureListener(Action<Java.Lang.Exception> action) => _action = action;
//        public void OnFailure(Java.Lang.Exception e) => _action(e);
//    }
//    #endif
//}
