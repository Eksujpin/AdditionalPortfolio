package dk.itu.moapd.scootersharing.fragments

import android.annotation.SuppressLint
import android.content.ContentValues
import android.content.Intent
import android.location.Address
import android.location.Geocoder
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.fragment.app.activityViewModels
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser
import com.google.zxing.integration.android.IntentIntegrator
import dk.itu.moapd.scootersharing.R
import dk.itu.moapd.scootersharing.activities.ScooterSharingActivity
import dk.itu.moapd.scootersharing.databinding.FragmentRideBinding
import dk.itu.moapd.scootersharing.models.*
import java.io.IOException
import java.util.*
import kotlin.collections.ArrayList
import kotlin.math.round


class RideFragment : Fragment() {
    private lateinit var qrScanIntegrator: IntentIntegrator
    private lateinit var auth: FirebaseAuth
    private lateinit var user: FirebaseUser
    private lateinit var ride: Ride
    private lateinit var scot: Scooter
    private val rideVM: RideVM by activityViewModels()
    private val scooterVM: ScooterVM by activityViewModels()

    private var _binding: FragmentRideBinding? = null
    private val binding get() = _binding!!


    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentRideBinding.inflate(inflater, container, false)
        auth = FirebaseAuth.getInstance()
        user = auth.currentUser!!

        initializeRide()

        binding.cancelReservation.setOnClickListener{
            if (this::ride.isInitialized && ride.rideStatus != runStatus.Running && ride.rideStatus != runStatus.Cancelled){
                ScooterSharingActivity.lastReserved = null
                ride.rideStatus = runStatus.Cancelled
                update()
            }else Toast.makeText(activity, "no ride reserved to cancel", Toast.LENGTH_SHORT).show()
        }

        binding.startRide.setOnClickListener{
            if(this::ride.isInitialized && ride.rideStatus == runStatus.Reserved){
                ride.rideStatus = runStatus.Running
                ride.rideStartTime = System.currentTimeMillis()
                ride.startLat = scooterVM.locationState.value!!.latitude
                ride.startLong = scooterVM.locationState.value!!.longitude
                update()
            }else Toast.makeText(activity, "No ride reserved to begin", Toast.LENGTH_SHORT).show()
        }

        binding.endRide.setOnClickListener{
            if (this::ride.isInitialized &&ride.rideStatus == runStatus.Running){
                ride.rideStatus = runStatus.Finished
                ride.rideEndTime = System.currentTimeMillis()
                ride.endLat = scooterVM.locationState.value!!.latitude
                ride.endLong = scooterVM.locationState.value!!.longitude
                ride.ridePrice = round((ride.distanceMeters()/100) * 100.0) / 100.0
                rideVM.insert(ride)

                scot.scooterLat = ride.endLat!!
                scot.scooterLong = ride.endLong!!
                scot.scooterDate = ride.rideEndTime!!
                scot.scooterLocation = getAddress(ride.endLat!!,ride.endLong!!)
                scot.scooterBattery = scot.scooterBattery - (ride.distanceMeters().toInt()/100)
                scooterVM.update(scot)
                update()
                ScooterSharingActivity.lastReserved = null
            }else Toast.makeText(activity, "No running ride to end", Toast.LENGTH_SHORT).show()
        }

        return binding.root
    }

    private fun initializeRide(){
        if (ScooterSharingActivity.lastReserved != null){
            val temp = ScooterSharingActivity.lastReserved!!
            val lat = scooterVM.locationState.value!!.latitude
            val long = scooterVM.locationState.value!!.longitude
            ride = if(this::ride.isInitialized){
                when (ride.rideStatus) {
                    runStatus.Running -> {
                        Ride(runStatus.Running,ride.rideScooterID,ride.rideUser,ride.startLat,ride.startLong,ride.rideStartTime,null,null,null,null)
                    }
                    else -> {
                        Ride(runStatus.Reserved,temp,user.uid,lat,long,System.currentTimeMillis(),null,null,null,null)
                    }
                }
            }else Ride(runStatus.Reserved,temp,user.uid,lat,long,System.currentTimeMillis(),null,null,null,null)
            update()
        }
    }

    @SuppressLint("SetTextI18n")
    fun update(){
        if(ScooterSharingActivity.lastReserved != null){
            scooterVM.getById(ScooterSharingActivity.lastReserved!!).observe(viewLifecycleOwner){
                scot = it
                binding.scooterName.text = it.scooterName
                binding.scooterBattery.text = it.scooterBattery.toString()
                binding.rideStatus.text = ride.rideStatus.toString()
                if (ride.rideStatus != runStatus.Finished){
                    binding.rideDistance.text = ""
                    binding.rentalTime.text = ""
                    binding.rentalPrice.text = ""
                }else{
                    binding.rideDistance.text = ride.distanceMeters().toString() + " Meters"
                    binding.rentalTime.text = ride.rideTime()
                    binding.rentalPrice.text = ride.ridePrice.toString() + " Kr"
                }
            }

        }else{
            binding.scooterName.text = ""
            binding.scooterBattery.text = ""
            binding.rideStatus.text = ""
            binding.rideDistance.text = ""
            binding.rentalTime.text = ""
            binding.rentalPrice.text = ""

        }

    }


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        //qr code scanner
        qrScanIntegrator = IntentIntegrator.forSupportFragment(this)
        qrScanIntegrator.setOrientationLocked(false)
        binding.btnScan.setOnClickListener { qrScanIntegrator.initiateScan() }
    }

    //handels result of QR scan
    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        val result = IntentIntegrator.parseActivityResult(requestCode, resultCode, data)
        if (result != null) {
            // If QRCode has no data.
            if (result.contents == null) {
                Toast.makeText(activity, R.string.result_not_found, Toast.LENGTH_LONG).show()
            } else { try {
                // If QRCode contains data.
                val out = result.contents.toInt()
                var match = false
                scooterVM.getAll().observe(viewLifecycleOwner, { data ->
                    val temp = data.toCollection(ArrayList())
                    for (scooter in temp){
                        if (scooter.id == out) match = true
                    }
                    if (match){
                        val lat = scooterVM.locationState.value!!.latitude
                        val long = scooterVM.locationState.value!!.longitude
                        ride = Ride(runStatus.Reserved,out,user.uid,lat,long,System.currentTimeMillis(),null,null,null,null)
                        ScooterSharingActivity.lastReserved = out
                        update()
                    }else{
                        Toast.makeText(activity, "No matching scooterID found " + result.contents, Toast.LENGTH_LONG).show()
                        ScooterSharingActivity.lastReserved = null
                        update()
                    }
                })
            }catch (e:NumberFormatException){
                e.printStackTrace()
                // Data not in the expected format. So, whole object as toast message.
                Toast.makeText(activity, "Data wasn't a number: " + result.contents, Toast.LENGTH_LONG).show()
            }
            }
        } else {
            super.onActivityResult(requestCode, resultCode, data)
        }
    }


    private fun getAddress(latitude: Double, longitude: Double): String {
        val geocoder = Geocoder(requireContext(), Locale.getDefault())
        try {
            val addresses = geocoder.getFromLocation(latitude, longitude, 1)
            return if (addresses.isNotEmpty()) {
                val address: Address = addresses[0]
                address.getAddressLine(0)
            } else "Address not found!"
        } catch (ex: IOException) { Log.e(ContentValues.TAG, ex.printStackTrace().toString()) }
        return "Address not found!"
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }

}




