import {
    useQuery, keepPreviousData
} from "@tanstack/react-query";
import { searchNodes } from "@/entities/imagenet/api/searchApi";
import type { SearchResponse } from "@/entities/imagenet/api/searchApi";
import { useDebounce } from "@/shared/lib/useDebounce";
import {
    SEARCH_PAGE_SIZE,
    SEARCH_DEBOUNCE_MS,
    DEFAULT_CACHE_TTL
} from "@/shared/config/constants";

export const useSearchNodes = (
    rawQuery: string,
    page: number,
    pageSize = SEARCH_PAGE_SIZE
) => {
    const debouncedQuery = useDebounce(rawQuery, SEARCH_DEBOUNCE_MS);

    return useQuery<SearchResponse, Error>({
        queryKey: ["search", debouncedQuery, page, pageSize],
        queryFn: () => searchNodes(debouncedQuery, page, pageSize),
        enabled: debouncedQuery.trim().length > 0,
        staleTime: DEFAULT_CACHE_TTL,
        placeholderData: keepPreviousData,
    });
};
