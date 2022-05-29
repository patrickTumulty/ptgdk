using UnityEngine;

namespace PTGDK.Utility
{
    public static class RandomUtils
    {   
        public static float GetRandomSymmetricalRange(float num)
        {
            return Random.Range(-num, num);
        }
        
        public static Vector3 GetRandomVector3(float x, float y, float z)
        {
            return new Vector3(GetRandomSymmetricalRange(x), 
                               GetRandomSymmetricalRange(y),
                               GetRandomSymmetricalRange(z));
        }
        
        public static Quaternion GetRandomQuaternion(float x, float y, float z, float w)
        {
            return new Quaternion(GetRandomSymmetricalRange(x), 
                                  GetRandomSymmetricalRange(y) , 
                                  GetRandomSymmetricalRange(z), 
                                  GetRandomSymmetricalRange(w));
        }
        
    }
}