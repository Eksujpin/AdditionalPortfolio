package dk.itu.moapd.scootersharing.fragments

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser
import com.google.firebase.auth.UserProfileChangeRequest
import dk.itu.moapd.scootersharing.R
import dk.itu.moapd.scootersharing.databinding.FragmentEditUserBinding


class EditUserFragment : Fragment() {

    private var _binding: FragmentEditUserBinding? = null
    private val binding get() = _binding!!
    private lateinit var auth: FirebaseAuth
    private lateinit var user: FirebaseUser

override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
    _binding = FragmentEditUserBinding.inflate(inflater, container, false)

    auth = FirebaseAuth.getInstance()
    user = auth.currentUser!!

    binding.infoText.text = user.displayName

    binding.updateUsernameBtn.setOnClickListener {
        if (binding.userNameEditText.text.isNotEmpty()){
            user.updateProfile(UserProfileChangeRequest.Builder().setDisplayName(binding.userNameEditText.text.toString().trim()).build())
            binding.infoText.text = binding.userNameEditText.text.toString().trim()
        }else Toast.makeText(this.requireContext(), "Plese enter a new username to update", Toast.LENGTH_SHORT).show()
    }

    binding.backBtn.setOnClickListener {
        activity!!.supportFragmentManager.beginTransaction().replace(R.id.fragment_container, UserProfileFragment()).commit()
    }






    return binding.root
    }






}
