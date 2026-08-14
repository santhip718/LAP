import HeroSection from "../../components/HeroSection/HeroSection";
import TrustedBySection from "../../components/TrustedBySection/TrustedBySection";
import FeaturesSection from "../../components/FeaturesSection/FeaturesSection";
import LapFooter from "@/shared/components/layout/LapFooter/LapFooter";
import "./Home.css";

export default function Home() {
  return (
    <>
      <div className="home">
        <HeroSection />
        <TrustedBySection />
        <FeaturesSection />
      </div>
      <LapFooter />
    </>
  );
}
