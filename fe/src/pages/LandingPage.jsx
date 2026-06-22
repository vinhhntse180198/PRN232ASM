import Navbar from '../components/landing/Navbar'
import Hero from '../components/landing/Hero'
import DashboardPreview from '../components/landing/DashboardPreview'
import HowItWorks from '../components/landing/HowItWorks'
import FeaturesGrid from '../components/landing/FeaturesGrid'
import Testimonials from '../components/landing/Testimonials'
import FAQ from '../components/landing/FAQ'
import CTASection from '../components/landing/CTASection'
import Footer from '../components/landing/Footer'

export default function LandingPage() {
  return (
    <div className="min-h-screen bg-base">
      <Navbar />
      <main>
        <Hero />
        <DashboardPreview />
        <HowItWorks />
        <FeaturesGrid />
        <Testimonials />
        <FAQ />
        <CTASection />
      </main>
      <Footer />
    </div>
  )
}
