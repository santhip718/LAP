import { useEffect, useRef } from "react";
import Typography from "@mui/material/Typography";
import CourseCard from "../../components/CourseCard/CourseCard";
import FilterBar from "../../components/FilterBar/FilterBar";
import LapNoContent from "@/shared/components/ui/LapNoContent/LapNoContent";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import { useEnrollment } from "@/core/providers/EnrollmentProvider";
import { useCourse } from "@/core/providers/CourseProvider";
import {
  HERO_TITLE,
  HERO_SUBTITLE,
  EMPTY_CONFIG,
  OBSERVER_ROOT_MARGIN,
  LOADING_LABEL,
  END_LABEL,
} from "./DiscoverCourses.constants";
import "./DiscoverCourses.css";

export default function DiscoverCourses() {
  const {
    courses,
    allLoaded,
    loading,
    initialized,
    loadInitial,
    loadMore,
    setFilters,
  } = useCourse();
  const sentinelRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!initialized) {
      loadInitial();
    }
  }, [initialized, loadInitial]);

  const loadMoreRef = useRef(loadMore);
  const loadingRef = useRef(loading);
  const allLoadedRef = useRef(allLoaded);

  useEffect(() => {
    loadMoreRef.current = loadMore;
  }, [loadMore]);

  useEffect(() => {
    loadingRef.current = loading;
  }, [loading]);

  useEffect(() => {
    allLoadedRef.current = allLoaded;
  }, [allLoaded]);

  const { enrolledCourses, enroll } = useEnrollment();
  const hasCourses = courses.length > 0;

  useEffect(() => {
    const el = sentinelRef.current;
    if (!el) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (
          entries[0].isIntersecting &&
          !loadingRef.current &&
          !allLoadedRef.current
        ) {
          loadMoreRef.current();
        }
      },
      { rootMargin: OBSERVER_ROOT_MARGIN },
    );

    observer.observe(el);
    return () => observer.disconnect();
  }, [hasCourses, allLoaded]);

  return (
    <div className="discover-page">
      <main className="discover-main">
        <section className="discover-hero">
          <Typography variant="h3" className="discover-hero-title">{HERO_TITLE}</Typography>
          <Typography variant="body1" className="discover-hero-subtitle">
            {HERO_SUBTITLE}
          </Typography>
          <FilterBar onFilterChange={setFilters} />
        </section>

        <div className="discover-grid">
          {!hasCourses && !loading && (
            <LapNoContent
              icon={EMPTY_CONFIG.icon}
              title={EMPTY_CONFIG.title}
              message={EMPTY_CONFIG.message}
            />
          )}
          {courses.map((course) => (
            <CourseCard
              key={course.id}
              course={course}
              enrollment={enrolledCourses[course.id]}
              onEnroll={enroll}
            />
          ))}
        </div>

        {!hasCourses && loading && (
          <div className="discover-sentinel">
            <LapSpinnerv1 />
          </div>
        )}

        {hasCourses && !allLoaded && (
          <div className="discover-sentinel" ref={sentinelRef}>
            {loading && (
              <>
                <LapSpinnerv1 />
                <Typography variant="body2">{LOADING_LABEL}</Typography>
              </>
            )}
          </div>
        )}

        {allLoaded && hasCourses && (
          <div className="discover-end">
            <Typography variant="body2">{END_LABEL}</Typography>
          </div>
        )}
      </main>
    </div>
  );
}
