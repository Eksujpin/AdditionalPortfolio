package dk.itu.moapd.scootersharing.models

import android.app.Application
import android.location.Location
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import kotlinx.coroutines.launch

class RideVM (application: Application) : AndroidViewModel(application) {
    private val rideRepository = RideRepository(application)
    private val data: LiveData<List<Ride>> = rideRepository.getAllRides()
    private val location = MutableLiveData<Location>()

    fun insert(ride: Ride) = viewModelScope.launch {
        rideRepository.insert(ride)
    }
    fun update(ride: Ride) = viewModelScope.launch {
        rideRepository.update(ride)
    }
    fun delete(ride: Ride) = viewModelScope.launch {
        rideRepository.delete(ride)
    }

    fun getAllRideByUser(userID: String) = rideRepository.getAllRideByUser(userID)

}