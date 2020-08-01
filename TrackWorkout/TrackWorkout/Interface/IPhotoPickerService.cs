using System;
using System.IO;
using System.Threading.Tasks;

namespace TrackWorkout.Interface
{
    public interface IPhotoPickerService
    {
        Task<Stream> GetImageStreamAsync();
    }
}
