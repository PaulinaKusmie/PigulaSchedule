using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigulaSchedule.Interface
{
    public interface IGeminiOcrService
    {
        Task<string> RecognizeScheduleAsync(byte[] imageBytes);
    }


}
