import { PageWrapper } from "@/components/layout/page-wrapper";
import {
  ClosingSoonSection,
  HomeHero,
  BrowseCategoriesSection,
  UpcomingAuctionsSection,
} from "@/features/auctions";

export default function Page() {
  return (
    <>
      <HomeHero />
      
      <PageWrapper className="py-0">
        {/* Closing Soon Section */}
        <ClosingSoonSection />
      </PageWrapper>

      {/* Browse Categories Section (Full-Bleed background) */}
      <BrowseCategoriesSection />

      <PageWrapper className="py-0">
        {/* Upcoming Auctions Section */}
        <UpcomingAuctionsSection />
      </PageWrapper>
    </>
  );
}
