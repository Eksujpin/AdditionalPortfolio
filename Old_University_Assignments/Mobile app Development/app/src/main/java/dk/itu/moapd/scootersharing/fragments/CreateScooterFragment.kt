package dk.itu.moapd.scootersharing.fragments

import android.content.ContentValues
import android.location.Address
import android.location.Geocoder
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.fragment.app.activityViewModels
import dk.itu.moapd.scootersharing.R
import dk.itu.moapd.scootersharing.databinding.FragmentCreateScooterBinding
import dk.itu.moapd.scootersharing.models.Scooter
import dk.itu.moapd.scootersharing.models.ScooterVM
import dk.itu.moapd.scootersharing.models.lockStatus
import java.io.IOException
import java.util.*

class CreateScooterFragment : Fragment() {
    // GUI variables
    private lateinit var nameText: EditText
    private lateinit var locationText: TextView
    private lateinit var spinner: Spinner

    private val scooterVM: ScooterVM by activityViewModels()

    private var _binding: FragmentCreateScooterBinding? = null
    private val binding get() = _binding!!


    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentCreateScooterBinding.inflate(inflater, container, false)

        // Edit texts
        nameText = binding.nameText
        spinner = binding.scooterSpinner
        locationText = binding.LocationText

        ArrayAdapter.createFromResource(binding.root.context, R.array.scooter_models,android.R.layout.simple_spinner_item)
            .also { adapter ->
                adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                spinner.adapter = adapter
            }
        val lat = scooterVM.locationState.value!!.latitude
        val long = scooterVM.locationState.value!!.longitude
        val address = getAddress(lat,long)
        locationText.text = address

        // Buttons
        binding.addBtn.setOnClickListener {
            if (nameText.text.isNotEmpty()) {
                // Update the object attributes .
                val name = nameText.text.toString().trim()
                val timestamp = System.currentTimeMillis()
                val imageURL = spinner.selectedItem.toString()+".png"
                val scooter = Scooter(name, lockStatus.Unlocked, 100 ,address, lat, long, timestamp, imageURL)
                scooterVM.insert(scooter)
                // Reset the text fields and update the UI.
                nameText.text.clear()
                activity!!.supportFragmentManager.beginTransaction().replace(R.id.fragment_container, ScooterListFragment()).commit()
            }else Toast.makeText(this.requireContext(), "Pleas fill all fields to start a new ride", Toast.LENGTH_SHORT).show()
        }
        return binding.root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
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

}