namespace RTSP_Timelapse_App
{
    public class FFmpegArguments
    {
        public class Default
        {
            public const string Y = "true";
            public const string RTSP = "false";
            public const string Resolution = "1280x720";
            public const string FPS = "24";
            public const string Framerate = "24";
            public const string VideoBitrate = "1000";
            public const string AudioBitrate = "128";
            public const string VideoCodec = "libx264";
            public const string AudioCodec = "aac";
            public const string PixelFormat = "yuv420p";
            public const string Frames = "1";
            public const string Time = "60";
        }

        private string y = string.Empty;
        public string Y
        {
            get { return y == "true" ? "-y " : ""; } 
            set { if (value == "true" || value == "false") y = value; }
        }

        private string rtsp = string.Empty;
            public string RTSP
        {
            get { return rtsp == "true" ? "-rtsp_transport tcp " : ""; }
            set { if (value == "true" || value == "false") rtsp = value; }
        }

        private string resolution = string.Empty;
        public string Resolution
        {
            get { return resolution != string.Empty ? $"-s {resolution} " : ""; }
            set
            {
                if (value.Split('x').Length == 2)
                {
                    bool isNumber = true;
                    foreach (char ch in value.Split('x')[0])
                        if (!char.IsDigit(ch))
                        {
                            isNumber = false;
                            break;
                        }
                    if (isNumber)
                        foreach (char ch in value.Split('x')[1])
                            if (!char.IsDigit(ch))
                            {
                                isNumber = false;
                                break;
                            }
                    if (isNumber)
                        resolution = value;
                }
            }
        }

        private string fps = string.Empty;
        public string FPS
        {
            get { return fps != string.Empty ? $"-r {fps} " : ""; }
            set
            {
                bool isNumber = true;
                foreach (char ch in value)
                    if (!char.IsDigit(ch))
                    {
                        isNumber = false;
                        break;
                    }
                if (isNumber)
                    fps = value;
            }
        }

        private string framerate = string.Empty;
        public string Framerate
        {
            get { return framerate != string.Empty ? $"-framerate {framerate} " : ""; }
            set
            {
                bool isNumber = true;
                foreach (char ch in value)
                    if (!char.IsDigit(ch))
                    {
                        isNumber = false;
                        break;
                    }
                if (isNumber)
                    framerate = value;
            }
        }

        private string videoBitrate = string.Empty;
        public string VideoBitrate
        {
            get { return videoBitrate != string.Empty ? $"-b:v {videoBitrate}k " : ""; }
            set
            {
                bool isNumber = true;
                foreach (char ch in value)
                    if (!char.IsDigit(ch))
                    {
                        isNumber = false;
                        break;
                    }
                if (isNumber)
                    videoBitrate = value;
            }
        }

        private string audioBitrate = string.Empty;
        public string AudioBitrate
        {
            get { return audioBitrate != string.Empty ? $"-b:a {audioBitrate}k " : ""; }
            set
            {
                bool isNumber = true;
                foreach (char ch in value)
                    if (!char.IsDigit(ch))
                    {
                        isNumber = false;
                        break;
                    }
                if (isNumber)
                    audioBitrate = value;
            }
        }

        private string videoCodec = string.Empty;
        public string VideoCodec
        {
            get { return videoCodec != string.Empty ? $"-c:v {videoCodec} " : ""; }
            set { videoCodec = value; }
        }

        private string audioCodec = string.Empty;
        public string AudioCodec
        {
            get { return audioCodec != string.Empty ? $"-c:a {audioCodec} " : ""; }
            set { audioCodec = value; }
        }

        private string pixelFormat = string.Empty;
        public string PixelFormat
        {
            get { return pixelFormat != string.Empty ? $"-vf format={pixelFormat} " : ""; }
            set { pixelFormat = value; }
        }

        private string frames = string.Empty;
        public string Frames
        {
            get { return frames != string.Empty ? $"-frames:v {frames} " : ""; }
            set
            {
                bool isNumber = true;
                foreach (char ch in value)
                    if (!char.IsDigit(ch))
                    {
                        isNumber = false;
                        break;
                    }
                if (isNumber)
                    frames = value;
            }
        }

        private string time = string.Empty;
        public string Time
        {
            get { return time != string.Empty ? $"-t {time} " : ""; }
            set
            {
                bool isNumber = true;
                foreach (char ch in value)
                    if (!char.IsDigit(ch))
                    {
                        isNumber = false;
                        break;
                    }
                if (isNumber)
                    time = value;
            }
        }

        private string acceleration = string.Empty;
        public string Acceleration
        {
            get { return acceleration != string.Empty ? $"-vf setpts=PTS/{acceleration} " : ""; }
            set
            {
                bool isNumber = true;
                foreach (char ch in value)
                    if (!char.IsDigit(ch))
                    {
                        isNumber = false;
                        break;
                    }
                if (isNumber)
                    acceleration = value;
            }
        }

        private string deceleration = string.Empty;
        public string Deceleration
        {
            get { return deceleration != string.Empty ? $"-vf setpts=PTS*{deceleration} " : ""; }
            set
            {
                bool isNumber = true;
                foreach (char ch in value)
                    if (!char.IsDigit(ch))
                    {
                        isNumber = false;
                        break;
                    }
                if (isNumber)
                    deceleration = value;
            }
        }

        private string noAudio = string.Empty;
        public string NoAudio
        {
            get { return noAudio == "true" ? "-an " : ""; } 
            set { if (value == "true" || value == "false") noAudio = value; }
        }

        private string concat = string.Empty;
        public string Concat
        {
            get { return concat == "true" ? "-f concat " : ""; } 
            set { if (value == "true" || value == "false") concat = value; }
        }
    }
}