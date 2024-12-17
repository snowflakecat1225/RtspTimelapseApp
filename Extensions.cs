namespace RTSP_Timelapse_App
{
    public static class Extensions
    {
        public static bool IsNumber(this string s)
        {
            foreach (var c in s)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }
    }
}
