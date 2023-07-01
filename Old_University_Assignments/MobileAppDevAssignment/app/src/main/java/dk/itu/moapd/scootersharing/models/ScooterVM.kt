package dk.itu.moapd.scootersharing.models

import android.app.Application
import android.location.Location
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch

class ScooterVM (application: Application) : AndroidViewModel(application) {
    private val scooterRepository = ScooterRepository(application)
    private val data: LiveData<List<Scooter>> = scooterRepository.getAllScooters()
    private val location = MutableLiveData<Location>()

    fun insert(scooter: Scooter) = viewModelScope.launch {
        scooterRepository.insert(scooter)
    }
    fun update(scooter: Scooter) = viewModelScope.launch {
        scooterRepository.update(scooter)
    }
    fun delete(scooter: Scooter) = viewModelScope.launch {
        scooterRepository.delete(scooter)
    }

    fun onLocationChanged(location: Location){
        this.location.value = location
    }

    val locationState:LiveData<Location>
        get() = location

    fun getAll(): LiveData<List<Scooter>> = data

    fun getById(id: Int) = scooterRepository.getById(id)

}